using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public class DomainEventSynchronizer
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStreamPositionRepository _acknowledger;
        private readonly EventStoreConfiguration _configuration;

        public DomainEventSynchronizer(IEventStore eventStore, IEventPublisher eventPublisher, IStreamPositionRepository acknowledger, EventStoreConfiguration configuration)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
            _acknowledger = acknowledger;
            _configuration = configuration;
        }

        public async Task StartSynchronization()
        {
            await _eventStore.Connect(
                _configuration.Server,
                _configuration.Port,
                _configuration.Login,
                _configuration.Password
            );

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