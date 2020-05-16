using System.Threading;
using System.Threading.Tasks;
using Sociomedia.Application.Domain;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public class DomainEventSynchronizer
    {
        private readonly IEventBus _eventBus;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStreamPositionRepository _streamPositionRepository;
        private readonly ProjectionSynchronizationConfiguration _configuration;

        public DomainEventSynchronizer(
            IEventBus eventBus,
            IEventPublisher eventPublisher,
            IStreamPositionRepository streamPositionRepository,
            ProjectionSynchronizationConfiguration configuration)
        {
            _eventBus = eventBus;
            _eventPublisher = eventPublisher;
            _streamPositionRepository = streamPositionRepository;
            _configuration = configuration;
        }

        public async Task StartSynchronization()
        {
            var lastPosition = await _streamPositionRepository.GetLastPosition();
            await _eventBus.StartListeningEvents(lastPosition, DomainEventReceived, Disconnected);
        }

        private async Task DomainEventReceived(long position, DomainEvent @event)
        {
            await _eventPublisher.Publish(@event);
            await _streamPositionRepository.Save(position);
        }

        private async Task Disconnected()
        {
            StopSynchronization();
            Thread.Sleep(_configuration.ReconnectionDelayMs);
            await StartSynchronization();
        }

        public void StopSynchronization()
        {
            _eventBus.StopListeningEvents();
        }
    }
}