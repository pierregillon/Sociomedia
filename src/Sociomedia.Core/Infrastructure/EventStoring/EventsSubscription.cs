using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public class EventsSubscription
    {
        private readonly DomainEventReceived _domainEventReceived;
        private readonly ILogger _logger;
        private StreamSubscription _subscription;
        private readonly Dictionary<string, Type> _nameToEventType;
        private readonly JsonSerializerSettings _serializerSettings;
        private Position _lastPosition;
        private EventStoreClient _currentClient;

        public EventsSubscription(long? initialPosition, IEnumerable<Type> eventTypes, DomainEventReceived domainEventReceived, ILogger logger)
        {
            _lastPosition = initialPosition.HasValue ? new Position((ulong) initialPosition.Value, (ulong) initialPosition.Value) : Position.Start;
            _domainEventReceived = domainEventReceived;
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

        public async Task DefineConnection(EventStoreClient client)
        {
            _currentClient = client;

            await Subscribe();
        }

        private async Task Subscribe()
        {
            if (_currentClient == null) {
                return;
            }

            _subscription?.Dispose();

            _subscription = await _currentClient.SubscribeToAllAsync(
                _lastPosition,
                EventAppeared,
                subscriptionDropped: SubscriptionDropped,
                filterOptions: new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents(), checkpointReached: (s, position, c) => {
                    _logger.LogDebug($"Checkpoint taken at {position.PreparePosition}");
                    _lastPosition = position;
                    return Task.CompletedTask;
                })
            );
        }

        public void Unsubscribe()
        {
            _subscription?.Dispose();
            _subscription = null;
        }


        // ----- Callback

        private async Task EventAppeared(StreamSubscription subscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
        {
            try {
                if (resolvedEvent.OriginalStreamId.StartsWith("$") || !TryConvertToDomainEvent(resolvedEvent, out var @event)) {
                    _lastPosition = resolvedEvent.OriginalPosition.GetValueOrDefault();
                    return;
                }

                Debug($"Event received : stream: {resolvedEvent.OriginalStreamId}, date: {resolvedEvent.Event.Created:g}, type: {@event.GetType().Name}");
                await _domainEventReceived.Invoke(@event, (long) resolvedEvent.OriginalPosition.GetValueOrDefault().CommitPosition);
                _lastPosition = resolvedEvent.OriginalPosition.GetValueOrDefault();
            }
            catch (Exception ex) {
                Error(ex);
                Environment.Exit(-10);
            }
        }

        private async void SubscriptionDropped(StreamSubscription subscription, SubscriptionDroppedReason reason, Exception? error)
        {
            Unsubscribe();
            Debug("Subscription dropped. Reason: " + reason);
            Error(error);
            Thread.Sleep(5000);
            await Subscribe();
        }

        private bool TryConvertToDomainEvent(ResolvedEvent @event, out IEvent result)
        {
            try {
                var json = Encoding.UTF8.GetString(@event.Event.Data.ToArray());
                if (_nameToEventType.TryGetValue(@event.Event.EventType, out var eventType)) {
                    var domainEvent = (IEvent) JsonConvert.DeserializeObject(json, eventType, _serializerSettings);
                    domainEvent.Version = (int) @event.OriginalEventNumber.ToInt64() + 1;
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
            _logger.LogDebug($"[{GetType().DisplayableName()}] {message}");
        }

        private void Error(Exception ex)
        {
            _logger.LogError(ex, $"[{GetType().DisplayableName()}] Unhandled exception");
        }
    }
}