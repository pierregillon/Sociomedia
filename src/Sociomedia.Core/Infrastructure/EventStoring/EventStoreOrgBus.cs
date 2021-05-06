using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public class EventStoreOrgBus : IEventBus
    {
        private readonly ILogger _logger;
        private readonly EventStoreClient _client;
        private readonly List<EventsSubscription> _subscriptions = new List<EventsSubscription>();

        public EventStoreOrgBus(EventStoreConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            var settings = EventStoreClientSettings.Create(configuration.ConnectionString);
            settings.ConnectionName = AppDomain.CurrentDomain.FriendlyName;
            _client = new EventStoreClient(settings);
        }

        public bool IsConnected => _client != null;

        public async Task SubscribeToEvents(long? initialPosition, IEnumerable<Type> eventTypes, DomainEventReceived domainEventReceived)
        {
            var subscription = new EventsSubscription(
                initialPosition,
                eventTypes,
                domainEventReceived,
                _logger
            );

            _subscriptions.Add(subscription);

            await subscription.DefineConnection(_client);
        }

        public void Stop()
        {
            foreach (var subscription in _subscriptions) {
                subscription.Unsubscribe();
            }
            _subscriptions.Clear();
            _client?.Dispose();
        }
    }
}