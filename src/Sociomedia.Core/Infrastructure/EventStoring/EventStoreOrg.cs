using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Sociomedia.Application.Infrastructure.EventStoring
{
    public class EventStoreOrg : IEventStore
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
                Debug($"{eventData.Type} stored.");
            }
        }

        // ----- Internal logic

        private async Task Connect()
        {
            if (_connection != null) {
                return;
            }
            _connection = EventStoreConnection.Create(_configuration.Uri, AppDomain.CurrentDomain.FriendlyName);
            _connection.Closed += (sender, args) => {
                Error("Connection closed : " + args.Reason);
                _connection = null;
            };
            await _connection.ConnectAsync();
        }

        private IEvent ConvertToDomainEvent(ResolvedEvent @event)
        {
            try {
                var json = Encoding.UTF8.GetString(@event.Event.Data);
                var type = _typeLocator.FindEventType(@event.Event.EventType);
                if (type == null) {
                    throw new Exception("Event is unknown, unable to correctly deserialize it.");
                }
                var domainEvent = (IEvent) JsonConvert.DeserializeObject(json, type, _serializerSettings);
                domainEvent.Version = (int) @event.OriginalEventNumber + 1;
                return domainEvent;
            }
            catch (Exception ex) {
                throw new Exception($"An error occurred while parsing event from event store. Stream: {@event.Event.EventStreamId}, Position: {@event.Event.EventNumber}", ex);
            }
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