using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public class EventsSubscription
    {
        private readonly DomainEventReceived _domainEventReceived;
        private readonly LiveProcessingStarted _liveProcessingStarted;
        private readonly ILogger _logger;
        private EventStoreAllCatchUpSubscription _subscription;
        private readonly Dictionary<string, Type> _nameToEventType;
        private readonly JsonSerializerSettings _serializerSettings;
        private Position? _lastPosition;
        private IEventStoreConnection _currentConnection;

        public EventsSubscription(long? initialPosition, IEnumerable<Type> eventTypes, DomainEventReceived domainEventReceived, LiveProcessingStarted liveProcessingStarted, ILogger logger)
        {
            _lastPosition = initialPosition.HasValue ? new Position(initialPosition.Value, initialPosition.Value) : (Position?) null;
            _domainEventReceived = domainEventReceived;
            _liveProcessingStarted = liveProcessingStarted;
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
            _currentConnection = connection;
            _currentConnection.Closed += ConnectionOnClosed;

            Subscribe();
        }

        private void Subscribe()
        {
            if (_currentConnection == null) {
                return;
            }

            _subscription?.Stop();

            _subscription = _currentConnection.SubscribeToAllFrom(
                _lastPosition,
                CatchUpSubscriptionSettings.Default,
                EventAppeared,
                LiveProcessingStarted,
                SubscriptionDropped
            );
        }

        public void Unsubscribe()
        {
            _subscription?.Stop();
            _subscription = null;
        }

        // ----- Callback

        private void ConnectionOnClosed(object sender, ClientClosedEventArgs e)
        {
            Unsubscribe();
            _currentConnection.Closed -= ConnectionOnClosed;
            _currentConnection = null;
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription obj)
        {
            Debug("Switched to live mode");
            _liveProcessingStarted?.Invoke();
        }

        private async Task EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            try {
                if (resolvedEvent.OriginalStreamId.StartsWith("$") || !TryConvertToDomainEvent(resolvedEvent, out var @event)) {
                    _lastPosition = resolvedEvent.OriginalPosition;
                    return;
                }

                Debug($"Event received : stream: {resolvedEvent.OriginalStreamId}, date: {resolvedEvent.Event.Created:g}, type: {@event.GetType().Name}");
                await _domainEventReceived.Invoke(@event, resolvedEvent.OriginalPosition.GetValueOrDefault().CommitPosition);
                _lastPosition = resolvedEvent.OriginalPosition;
            }
            catch (Exception ex) {
                Error(ex);
                Environment.Exit(-10);
            }
        }

        private void SubscriptionDropped(EventStoreCatchUpSubscription _, SubscriptionDropReason reason, Exception error)
        {
            Unsubscribe();
            Debug("Subscription dropped. Reason: " + reason);
            Error(error);
            Thread.Sleep(5000);
            Subscribe();
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
            _logger.Debug($"[{GetType().DisplayableName()}] {message}");
        }

        private void Error(Exception ex)
        {
            _logger.Error(ex, $"[{GetType().DisplayableName()}] Unhandled exception");
        }
    }
}