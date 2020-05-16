using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public class EventsSubscription
    {
        private readonly DomainEventReceived _domainEventReceived;
        private readonly PositionInStreamChanged _positionInStreamChanged;
        private readonly ILogger _logger;
        private EventStoreAllCatchUpSubscription _subscription;
        private readonly Dictionary<string, Type> _nameToEventType;
        private readonly JsonSerializerSettings _serializerSettings;
        private Position? _lastPosition;

        public EventsSubscription(long? initialPosition, IEnumerable<Type> eventTypes, DomainEventReceived domainEventReceived, PositionInStreamChanged positionInStreamChanged, ILogger logger)
        {
            _lastPosition = initialPosition.HasValue ? new Position(initialPosition.Value, initialPosition.Value) : (Position?) null;
            _domainEventReceived = domainEventReceived;
            _positionInStreamChanged = positionInStreamChanged;
            _logger = logger;
            _nameToEventType = eventTypes.ToDictionary(x => x.Name);

            var jsonResolver = new PropertyCleanerSerializerContractResolver();
            jsonResolver.IgnoreProperty(typeof(IEvent), "Version");
            jsonResolver.RenameProperty(typeof(IEvent), "Id", "AggregateId");

            _serializerSettings = new JsonSerializerSettings {
                ContractResolver = jsonResolver,
                Formatting = Formatting.Indented
            };
        }

        public void DefineConnection(IEventStoreConnection connection)
        {
            connection.Closed += ConnectionOnClosed;

            _subscription?.Stop();

            _subscription = connection.SubscribeToAllFrom(
                _lastPosition,
                CatchUpSubscriptionSettings.Default,
                EventAppeared,
                LiveProcessingStarted
            );
        }

        public void Stop()
        {
            _subscription?.Stop();
        }

        // ----- Callback

        private void ConnectionOnClosed(object? sender, ClientClosedEventArgs e)
        {
            _subscription?.Stop();
            _subscription = null;
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription obj)
        {
            Debug("Switched to live mode");
        }

        private async Task EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            _lastPosition = resolvedEvent.OriginalPosition;

            if (resolvedEvent.OriginalStreamId.StartsWith("$")) {
                return;
            }

            if (TryConvertToDomainEvent(resolvedEvent, out var @event)) {
                Debug($"Event received : stream: {resolvedEvent.OriginalStreamId}, date: {resolvedEvent.Event.Created:g} type: {@event.GetType().Name}");
                if (_positionInStreamChanged != null) {
                    await _positionInStreamChanged.Invoke(_lastPosition.GetValueOrDefault().CommitPosition);
                }
                await _domainEventReceived.Invoke(@event);
            }
        }

        private bool TryConvertToDomainEvent(ResolvedEvent @event, out IEvent result)
        {
            try {
                var json = Encoding.UTF8.GetString(@event.Event.Data);
                if (_nameToEventType.TryGetValue(@event.Event.EventType, out var eventType)) {
                    var domainEvent = (IEvent) JsonConvert.DeserializeObject(json, eventType, _serializerSettings);
                    domainEvent.Version = (int) @event.OriginalEventNumber + 1;
                    result = domainEvent;
                    return true;
                }

                result = null;
                return false;
            }
            catch (Exception ex) {
                throw new Exception($"An error occurred while parsing event from event store. Stream: {@event.Event.EventStreamId}, Position: {@event.Event.EventNumber}", ex);
            }
        }

        private void Debug(string message)
        {
            _logger.Debug("[EVENTS_SUBSCRIPTION] " + message);
        }
    }
}