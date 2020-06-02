using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;

namespace Sociomedia.Core.Infrastructure.CQRS {
    public class ChainedEventPublisherDecorator : IEventPublisher
    {
        private readonly IEventPublisher _eventPublisher;
        private static readonly  ConcurrentQueue<IEvent> _eventQueue = new ConcurrentQueue<IEvent>();
        private static bool _publishing = false;

        public ChainedEventPublisherDecorator(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task Publish<T>(T @event, CancellationToken cancellationToken = new CancellationToken()) where T : class, IEvent
        {
            if (_publishing) {
                _eventQueue.Enqueue(@event);
            }
            else {
                try {
                    _publishing = true;
                    await _eventPublisher.Publish(@event, cancellationToken);
                    while (_eventQueue.Count > 0) {
                        if (_eventQueue.TryDequeue(out var value)) {
                            await _eventPublisher.Publish(value, cancellationToken);
                        }
                        else {
                            await Task.Delay(100, cancellationToken);
                        }
                    }
                }
                finally {
                    _publishing = false;
                }
            }
        }
    }
}