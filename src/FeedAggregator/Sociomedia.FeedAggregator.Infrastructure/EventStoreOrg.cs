using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using ILogger = Sociomedia.FeedAggregator.Infrastructure.Logging.ILogger;

namespace Sociomedia.FeedAggregator.Infrastructure
{
    public class EventStoreOrg : IEventStore
    {
        private const int EVENT_COUNT = 200;

        private IEventStoreConnection _connection;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger _logger;
        private readonly ITypeLocator _typeLocator;
        private readonly IEventPublisher _eventPublisher;

        public EventStoreOrg(ILogger logger, ITypeLocator typeLocator, IEventPublisher eventPublisher)
        {
            _logger = logger;
            _typeLocator = typeLocator;
            _eventPublisher = eventPublisher;

            var jsonResolver = new PropertyCleanerSerializerContractResolver();
            jsonResolver.IgnoreProperty(typeof(IEvent), "Version", "TimeStamp");
            jsonResolver.RenameProperty(typeof(IEvent), "Id", "AggregateId");

            _serializerSettings = new JsonSerializerSettings {
                ContractResolver = jsonResolver,
                Formatting = Formatting.Indented
            };
        }

        // ----- Public methods

        public async Task Connect(string server, int port = 1113, string login = "admin", string password = "changeit")
        {
            _connection = EventStoreConnection.Create(new Uri($"tcp://{login}:{password}@{server}:{port}"), "Aggregator");
            await _connection.ConnectAsync();
        }

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default(CancellationToken))
        {
            var streamEvents = await ReadAllEventsInStream(aggregateId.ToString(), fromVersion);

            return streamEvents
                .Select(ConvertToDomainEvent)
                .ToArray();
        }

        public async Task<IReadOnlyCollection<IEvent>> ReadAllEventsFromBeginning()
        {
            var streamEvents = await ReadAllEvents();

            return streamEvents
                .Where(x => x.OriginalStreamId.StartsWith("$") == false)
                .Select(TryConvertToDomainEvent)
                .Where(x => x != null)
                .ToArray();
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken))
        {
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
                await _eventPublisher.Publish(@event, cancellationToken);

            }
        }

        // ----- Internal logic

        private IEvent ConvertToDomainEvent(ResolvedEvent @event)
        {
            var json = Encoding.UTF8.GetString(@event.Event.Data);
            var type = _typeLocator.FindEventType(@event.Event.EventType);
            if (type == null) {
                throw new UnknownEvent(@event.Event.EventType);
            }
            var domainEvent = (IEvent) JsonConvert.DeserializeObject(json, type, _serializerSettings);
            domainEvent.Version = (int) @event.OriginalEventNumber + 1;
            return domainEvent;
        }

        private IEvent TryConvertToDomainEvent(ResolvedEvent @event)
        {
            try {
                return ConvertToDomainEvent(@event);
            }
            catch (UnknownEvent ex) {
                _logger.LogError(ex);
                return null;
            }
        }

        private async Task<IEnumerable<ResolvedEvent>> ReadAllEventsInStream(string streamId, int fromVersion)
        {
            var streamEvents = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            var nextSliceStart = fromVersion == -1 ? StreamPosition.Start : (long)fromVersion;

            do {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(streamId, nextSliceStart, EVENT_COUNT, false);
                nextSliceStart = currentSlice.NextEventNumber;
                streamEvents.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);

            return streamEvents;
        }

        private async Task<IEnumerable<ResolvedEvent>> ReadAllEvents()
        {
            var streamEvents = new List<ResolvedEvent>();
            AllEventsSlice currentSlice;
            var nextSliceStart = Position.Start;

            do {
                currentSlice = await _connection.ReadAllEventsForwardAsync(nextSliceStart, EVENT_COUNT, false);
                nextSliceStart = currentSlice.NextPosition;
                streamEvents.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);

            return streamEvents;
        }
    }
}