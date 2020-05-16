using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Sociomedia.Application.Infrastructure.EventStoring
{
    public class EventStoreOrgBus : IEventBus
    {
        private readonly EventStoreConfiguration _configuration;
        private readonly ILogger _logger;
        private IEventStoreConnection _connection;
        private readonly List<EventsSubscription> _subscriptions = new List<EventsSubscription>();

        public EventStoreOrgBus(EventStoreConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool IsConnected => _connection != null;

        public async Task SubscribeToEvents(long? initialPosition, IEnumerable<Type> eventTypes, DomainEventReceived domainEventReceived, PositionInStreamChanged positionInStreamChanged)
        {
            var subscription = new EventsSubscription(
                initialPosition,
                eventTypes,
                domainEventReceived,
                positionInStreamChanged,
                _logger
            );

            _subscriptions.Add(subscription);

            subscription.DefineConnection(await GetOrCreateNewConnection());
        }

        public void Stop()
        {
            foreach (var subscription in _subscriptions) {
                subscription.Stop();
            }
            _subscriptions.Clear();
            _connection?.Close();
            _connection = null;
        }

        private async Task<IEventStoreConnection> GetOrCreateNewConnection()
        {
            if (_connection != null) {
                return _connection;
            }
            return await CreateNewConnection();
        }

        private async Task<IEventStoreConnection> CreateNewConnection()
        {
            Info("Connecting to event bus ...");

            _connection = EventStoreConnection.Create(_configuration.Uri, AppDomain.CurrentDomain.FriendlyName);
            _connection.Closed += async (sender, args) => {
                Info("Connection closed : " + args.Reason);
                _connection = null;
                await TryToReconnect();
            };
            await _connection.ConnectAsync();
            return _connection;
        }

        private async Task TryToReconnect()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));

            var connection = await CreateNewConnection();

            foreach (var subscription in _subscriptions) {
                subscription.DefineConnection(connection);
            }
        }

        private void Info(string message)
        {
            _logger.Info("[EVENT_BUS] " + message);
        }
    }
}