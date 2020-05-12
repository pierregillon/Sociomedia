using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Sociomedia.Infrastructure
{
    public class EventStoreOrg : IEventStore
    {
        private const int EVENT_COUNT = 200;

        private IEventStoreConnection _connection;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly EventStoreConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly ITypeLocator _typeLocator;
        private Func<Task> _disconnected;
        private EventStoreAllCatchUpSubscription _subscription;
        private DomainEventReceived _domainEventReceived;

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

        public bool IsConnected => _connection != null;

        // ----- Public methods

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Connect();

            var streamEvents = await ReadAllEventsInStream(aggregateId.ToString(), fromVersion);

            return streamEvents
                .Select(ConvertToDomainEvent)
                .ToArray();
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken))
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

        public async Task SubscribeToEventsFrom(long? position, DomainEventReceived domainEventReceived, Func<Task> disconnected)
        {
            await Connect();

            _disconnected = disconnected;
            _domainEventReceived = domainEventReceived;

            _subscription = _connection.SubscribeToAllFrom(
                position.HasValue ? new Position(position.Value, position.Value) : (Position?) null,
                CatchUpSubscriptionSettings.Default,
                EventAppeared,
                LiveProcessingStarted,
                SubscriptionDropped
            );

            Debug($"Subscribing to all events, from position { position } ...");
        }

        // ----- Callback

        private void SubscriptionDropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex)
        {
            Error($"Subscription dropped : {reason}.");
            Error(ex);

            _disconnected.Invoke().Wait();
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription obj)
        {
            Debug("Switched to live mode");
        }

        private async Task EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.OriginalStreamId.StartsWith("$")) {
                return;
            }

            try {
                var position = resolvedEvent.OriginalPosition.GetValueOrDefault().CommitPosition;
                if (TryConvertToDomainEvent(resolvedEvent, out var @event)) {
                    Debug($"{resolvedEvent.Event.EventType} received. Stream: {resolvedEvent.Event.EventStreamId}, position: {position}");
                    await _domainEventReceived.Invoke(position, @event);
                }
                else {
                    Debug($"[UNKNOWN EVENT] {resolvedEvent.Event.EventType} received. Stream: {resolvedEvent.Event.EventStreamId}, position: {position}");
                }
            }
            catch (Exception ex) {
                Error(ex);
                throw;
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
                _connection = null;
            };
            await _connection.ConnectAsync();
        }

        private bool TryConvertToDomainEvent(ResolvedEvent @event, out IEvent result)
        {
            result = ConvertToDomainEvent(@event);
            return result != null;
        }

        private IEvent ConvertToDomainEvent(ResolvedEvent @event)
        {
            try {
                var json = Encoding.UTF8.GetString(@event.Event.Data);
                var type = _typeLocator.FindEventType(@event.Event.EventType);
                if (type == null) {
                    return null;
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

        private void Error(Exception ex)
        {
            _logger.Error(ex, "[EVENTSTORE]");
        }
    }

    public delegate Task DomainEventReceived(long position, IEvent @event);
}