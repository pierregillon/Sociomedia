using System.Threading.Tasks;

namespace NewsAggregator.ReadDatabaseSynchronizer
{
    public class DomainEventSynchronizer
    {
        private readonly EventStoreOrg _eventStore;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEventAcknowledger _acknowledger;

        public DomainEventSynchronizer(EventStoreOrg eventStore, IEventPublisher eventPublisher, IEventAcknowledger acknowledger)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
            _acknowledger = acknowledger;
        }

        public async Task StartSynchronization()
        {
            await _eventStore.Connect("localhost");

            var lastPosition = await _acknowledger.GetLastPosition();

            _eventStore.StartListeningEvents(lastPosition, async (position, @event) => {
                await _eventPublisher.Publish(@event);
                await _acknowledger.Acknowledge(position);
            });
        }

        public void StopSynchronization()
        {
            _eventStore.StopListeningEvents();
        }
    }
}