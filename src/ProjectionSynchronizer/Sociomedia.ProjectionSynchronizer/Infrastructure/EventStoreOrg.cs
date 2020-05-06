using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using Newtonsoft.Json;
using Sociomedia.DomainEvents;
using Sociomedia.DomainEvents.Article;
using Sociomedia.ProjectionSynchronizer.Application;

namespace Sociomedia.ProjectionSynchronizer.Infrastructure
{
    public class EventStoreOrg : IEventBus
    {
        private IEventStoreConnection _connection;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly EventStoreConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IDomainEventTypeLocator _typeLocator;
        private DomainEventReceived _onEventReceived;
        private EventStoreStreamCatchUpSubscription _subscription;
        private Func<Task> _disconnected;

        public EventStoreOrg(EventStoreConfiguration configuration, ILogger logger, IDomainEventTypeLocator typeLocator)
        {
            _configuration = configuration;
            _logger = logger;
            _typeLocator = typeLocator;

            var jsonResolver = new PropertyCleanerSerializerContractResolver();
            jsonResolver.IgnoreProperty(typeof(DomainEvent), "Version", "TimeStamp");
            jsonResolver.RenameProperty(typeof(DomainEvent), "Id", "AggregateId");

            _serializerSettings = new JsonSerializerSettings {
                ContractResolver = jsonResolver,
                Formatting = Formatting.Indented
            };
        }

        // ----- Public methods

        public async Task StartListeningEvents(long? lastPosition, DomainEventReceived onEventReceived, Func<Task> disconnected)
        {
            if (_connection != null) {
                throw new InvalidOperationException("Cannot listen events : already listening !");
            }

            _connection = EventStoreConnection.Create(_configuration.Uri, Assembly.GetExecutingAssembly().GetName().Name);

            await _connection.ConnectAsync();

            _onEventReceived = onEventReceived;
            _disconnected = disconnected;

            _subscription = _connection.SubscribeToStreamFrom("$et-" + typeof(ArticleSynchronized).Name, lastPosition, CatchUpSubscriptionSettings.Default, EventAppeared, LiveProcessingStarted, SubscriptionDropped);

            _logger.Debug($"Subscribed from position {lastPosition}. Replaying missing events.");
        }

        public void StopListeningEvents()
        {
            if (_subscription == null) {
                throw new InvalidOperationException("Subscription not started");
            }
            _subscription.Stop();
            _subscription = null;
            _connection.Close();
            _connection = null;
        }

        // ----- Internal logic

        private void SubscriptionDropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex)
        {
            _logger.Debug($"Subscription dropped : {reason}.");

            if (ex is ConnectionClosedException) {
                _logger.Error(ex.Message);
                _disconnected.Invoke().Wait();
            }
            else {
                _logger.Error(ex, string.Empty);
            }
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription obj)
        {
            _logger.Debug("Switched to live mode");
        }

        private async Task EventAppeared(EventStoreCatchUpSubscription arg1, ResolvedEvent evt)
        {
            try {
                _logger.Debug($"New event received. Stream : {evt.Event.EventStreamId}, position: {evt.OriginalEventNumber}");
                if (TryConvertToDomainEvent(evt, out var @event)) {
                    await _onEventReceived(evt.OriginalEventNumber, @event);
                }
            }
            catch (Exception ex) {
                _logger.Error(ex.Message);
                throw;
            }
        }

        private bool TryConvertToDomainEvent(ResolvedEvent @event, out DomainEvent result)
        {
            try {
                result = ConvertToDomainEvent(@event);
                return true;
            }
            catch (UnknownEvent ex) {
                _logger.Error(ex.Message);
                result = null;
                return false;
            }
        }

        private DomainEvent ConvertToDomainEvent(ResolvedEvent @event)
        {
            var json = Encoding.UTF8.GetString(@event.Event.Data);
            var type = _typeLocator.FindEventType(@event.Event.EventType);
            if (type == null) {
                throw new UnknownEvent(@event.Event.EventType);
            }
            return (DomainEvent) JsonConvert.DeserializeObject(json, type, _serializerSettings);
        }
    }
}