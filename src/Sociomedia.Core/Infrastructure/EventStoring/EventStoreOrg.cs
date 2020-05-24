using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Sociomedia.Core.Domain;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public class EventStoreOrg : IEventStore, IEventStoreExtended
    {
        private const int EVENT_COUNT = 200;

        private IEventStoreConnection _connection;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly EventStoreConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly ITypeLocator _typeLocator;

        public EventStoreOrg(EventStoreConfiguration configuration, ILogger logger, ITypeLocator typeLocator)
        {
            _configuration = configuration;
            _logger = logger;
            _typeLocator = typeLocator;

            var jsonResolver = new PropertyCleanerSerializerContractResolver();
            jsonResolver.IgnoreProperty(typeof(IEvent), "Version");
            jsonResolver.RenameProperty(typeof(IEvent), "Id", "AggregateId");

            _serializerSettings = new JsonSerializerSettings {
                ContractResolver = jsonResolver,
                Formatting = Formatting.Indented
            };
        }

        // ----- Public methods

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            await Connect();

            var streamEvents = await ReadAllEventsInStream(aggregateId.ToString(), fromVersion);

            return streamEvents
                .Select(ConvertToDomainEvent)
                .ToArray();
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
        {
            await Connect();

            foreach (var @event in events) {
                var json = JsonConvert.SerializeObject(@event, _serializerSettings);
                var eventData = new EventData(
                    Guid.NewGuid(),
                    @event.GetType().Name,
                    true,
                    Encoding.UTF8.GetBytes(json),
                    null
                );
                var version = @event.Version - 2; // CQRSLite start event version at 1. EventStore at -1.
                await _connection.AppendToStreamAsync(@event.Id.ToString(), version, eventData);
            }
        }

        public async IAsyncEnumerable<IEvent> GetAllEventsBetween(Position startPosition, Position endPosition, IReadOnlyCollection<Type> eventTypes)
        {
            await Connect();

            var eventTypesByName = eventTypes.ToDictionary(x => x.Name);

            AllEventsSlice currentSlice;

            do {
                currentSlice = await _connection.ReadAllEventsForwardAsync(startPosition, EVENT_COUNT, false);
                startPosition = currentSlice.NextPosition;
                foreach (var @event in currentSlice.Events.Where(x => !x.Event.EventType.StartsWith("$"))) {
                    if (eventTypesByName.TryGetValue(@event.Event.EventType, out var eventType)) {
                        yield return ConvertToDomainEvent(@event, eventType);
                    }
                    if (@event.OriginalPosition == endPosition) {
                        yield break;
                    }
                }
            } while (!currentSlice.IsEndOfStream);
        }

        public async Task<long> GetCurrentGlobalStreamPosition()
        {
            await Connect();

            var slice = await _connection.ReadAllEventsBackwardAsync(Position.End, 1, false);
            return slice.FromPosition.CommitPosition;
        }

        // ----- Internal logic

        private async Task Connect()
        {
            if (_connection != null) {
                return;
            }
            _connection = EventStoreConnection.Create(_configuration.Uri, AppDomain.CurrentDomain.FriendlyName);
            _connection.ErrorOccurred += (sender, args) => Error(args.Exception.ToString());
            _connection.Closed += (sender, args) => {
                Error("Connection closed : " + args.Reason);
                _connection = null;
            };
            await _connection.ConnectAsync();
        }

        private IEvent ConvertToDomainEvent(ResolvedEvent @event)
        {
            try {
                var type = _typeLocator.FindEventType(@event.Event.EventType);
                if (type == null) {
                    throw new Exception("Event is unknown, unable to correctly deserialize it.");
                }
                return ConvertToDomainEvent(@event, type);
            }
            catch (Exception ex) {
                throw new Exception($"An error occurred while parsing event from event store. Stream: {@event.Event.EventStreamId}, Position: {@event.Event.EventNumber}", ex);
            }
        }

        private IEvent ConvertToDomainEvent(ResolvedEvent @event, Type eventType)
        {
            var json = Encoding.UTF8.GetString(@event.Event.Data);
            var domainEvent = (IEvent) JsonConvert.DeserializeObject(json, eventType, _serializerSettings);
            domainEvent.Version = (int) @event.OriginalEventNumber + 1;
            return domainEvent;
        }

        private async Task<IEnumerable<ResolvedEvent>> ReadAllEventsInStream(string streamId, int fromVersion)
        {
            var streamEvents = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            var nextSliceStart = fromVersion == -1 ? StreamPosition.Start : (long) fromVersion;

            do {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(streamId, nextSliceStart, EVENT_COUNT, false);
                nextSliceStart = currentSlice.NextEventNumber;
                streamEvents.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);

            return streamEvents;
        }

        private void Debug(string message)
        {
            _logger.Debug("[EVENTSTORE] " + message);
        }

        private void Error(string message)
        {
            _logger.Error("[EVENTSTORE] " + message);
        }
    }
}