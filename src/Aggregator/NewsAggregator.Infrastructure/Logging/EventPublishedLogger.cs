using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;

namespace NewsAggregator.Infrastructure.Logging
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
            var watch = Stopwatch.StartNew();
            try {
                await _eventPublisher.Publish(@event, cancellationToken);
            }
            finally {
                await _logger.LogInformation(@event.GetType().Name, watch.ElapsedMilliseconds);
            }
        }
    }
}