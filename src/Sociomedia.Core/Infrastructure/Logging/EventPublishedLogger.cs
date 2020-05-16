using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;

namespace Sociomedia.Application.Infrastructure.Logging
{
    public class EventPublishedLogger : IEventPublisher
    {
        private readonly ILogger _logger;
        private readonly IEventPublisher _eventPublisher;

        public EventPublishedLogger(ILogger logger, IEventPublisher eventPublisher)
        {
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task Publish<T>(T @event, CancellationToken cancellationToken = new CancellationToken()) where T : class, IEvent
        {
            _logger.Debug(@event.GetType().Name);
            await _eventPublisher.Publish(@event, cancellationToken);
        }
    }
}