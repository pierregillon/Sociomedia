using System.Threading.Tasks;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public class DomainEventSynchronizer
    {
        private readonly IEventBus _eventBus;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStreamPositionRepository _streamPositionRepository;

        public DomainEventSynchronizer(IEventBus eventBus, IEventPublisher eventPublisher, IStreamPositionRepository streamPositionRepository)
        {
            _eventBus = eventBus;
            _eventPublisher = eventPublisher;
            _streamPositionRepository = streamPositionRepository;
        }

        public async Task StartSynchronization()
        {
            var lastPosition = await _streamPositionRepository.GetLastPosition();

            await _eventBus.StartListeningEvents(lastPosition, async (position, @event) => {
                await _eventPublisher.Publish(@event);
                await _streamPositionRepository.Save(position);
            });
        }

        public void StopSynchronization()
        {
            _eventBus.StopListeningEvents();
        }
    }
}