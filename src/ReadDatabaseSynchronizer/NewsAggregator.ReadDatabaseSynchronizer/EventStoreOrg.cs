using System;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace NewsAggregator.ReadDatabaseSynchronizer
{
    public class EventStoreOrg
    {
        private IEventStoreConnection _connection;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger _logger;
        private readonly ITypeLocator _typeLocator;
        private readonly IEventPublisher _eventPublisher;
        private EventStoreAllCatchUpSubscription _subscription;

        public EventStoreOrg(ILogger logger, ITypeLocator typeLocator, IEventPublisher eventPublisher)
        {
            _logger = logger;
            _typeLocator = typeLocator;
            _eventPublisher = eventPublisher;

            var jsonResolver = new PropertyCleanerSerializerContractResolver();
            jsonResolver.IgnoreProperty(typeof(IDomainEvent), "Version", "TimeStamp");
            jsonResolver.RenameProperty(typeof(IDomainEvent), "Id", "AggregateId");

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

        public void StartListeningEvents(Position? position)
        {
            _subscription = _connection.SubscribeToAllFrom(position, CatchUpSubscriptionSettings.Default, EventAppeared, LiveProcessingStarted, SubscriptionDropped);
        }

        public void StopListeningEvents()
        {
            if (_subscription == null) {
                throw new InvalidOperationException("Subscription not started");
            }
            _subscription.Stop();
        }

        // ----- Internal logic

        private void SubscriptionDropped(EventStoreCatchUpSubscription arg1, SubscriptionDropReason arg2, Exception arg3)
        {
            _logger.Debug("Subscription dropped");
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription obj)
        {
            _logger.Debug("Switched to live mode");
        }

        private async Task EventAppeared(EventStoreCatchUpSubscription arg1, ResolvedEvent evt)
        {
            if (!evt.OriginalStreamId.StartsWith("$")) {
                _logger.Debug($"New event received : {evt.OriginalStreamId}");
                if (TryConvertToDomainEvent(evt, out var @event)) {
                    await _eventPublisher.Publish(@event);
                }
            }
        }

        private bool TryConvertToDomainEvent(ResolvedEvent @event, out IDomainEvent result)
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

        private IDomainEvent ConvertToDomainEvent(ResolvedEvent @event)
        {
            var json = Encoding.UTF8.GetString(@event.Event.Data);
            var type = _typeLocator.FindEventType(@event.Event.EventType);
            if (type == null) {
                throw new UnknownEvent(@event.Event.EventType);
            }
            return (IDomainEvent) JsonConvert.DeserializeObject(json, type, _serializerSettings);
        }
    }
}