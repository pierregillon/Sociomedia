using System.Threading.Tasks;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public class DomainEventSynchronizer
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStreamPositionRepository _streamPositionRepository;
        private readonly EventStoreConfiguration _configuration;

        public DomainEventSynchronizer(IEventStore eventStore, IEventPublisher eventPublisher, IStreamPositionRepository streamPositionRepository, EventStoreConfiguration configuration)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
            _streamPositionRepository = streamPositionRepository;
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

            var lastPosition = await _streamPositionRepository.GetLastPosition();

            _eventStore.StartListeningEvents(lastPosition, async (position, @event) => {
                await _eventPublisher.Publish(@event);
                await _streamPositionRepository.Save(position);
            });
        }


        public void StopSynchronization()
        {
            _eventStore.StopListeningEvents();
        }
    }

    public delegate Task DomainEventReceived(long position, IDomainEvent @event);
}