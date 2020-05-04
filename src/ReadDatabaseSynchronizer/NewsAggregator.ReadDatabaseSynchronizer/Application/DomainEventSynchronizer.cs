using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace NewsAggregator.ReadDatabaseSynchronizer.Application
{
    public class DomainEventSynchronizer
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStreamPositionRepository _acknowledger;

        public DomainEventSynchronizer(IEventStore eventStore, IEventPublisher eventPublisher, IStreamPositionRepository acknowledger)
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
                await _acknowledger.Save(position);
            });
        }


        public void StopSynchronization()
        {
            _eventStore.StopListeningEvents();
        }
    }

    public delegate Task DomainEventReceived(Position position, IDomainEvent @event);
}