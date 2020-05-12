using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using Sociomedia.FeedAggregator.Application.SynchronizeAllMediaFeeds;
using Sociomedia.Infrastructure;
using Sociomedia.Infrastructure.CQRS;

namespace Sociomedia.FeedAggregator
{
    public class Aggregator
    {
        private readonly EventStoreOrg _eventStore;
        private readonly Configuration _configuration;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IEventPublisher _eventPublisher;
        private long? _lastPosition;

        public Aggregator(
            EventStoreOrg eventStore,
            Configuration configuration,
            ICommandDispatcher commandDispatcher,
            IEventPublisher eventPublisher)
        {
            _eventStore = eventStore;
            _configuration = configuration;
            _commandDispatcher = commandDispatcher;
            _eventPublisher = eventPublisher;
        }

        public Task StartAggregation(CancellationToken token)
        {
            return Task.Factory.StartNew(async () => {
                await _eventStore.SubscribeToEventsFrom(_lastPosition, DomainEventReceived, Disconnected);

                try {
                    do
                    {
                        await Task.Delay(_configuration.FeedAggregator.SynchronizationTimespan, token);
                        if (_eventStore.IsConnected) {
                            await _commandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());
                        }
                    } while (true);
                }
                catch (TaskCanceledException) {}

            }, token);
        }

        private async Task DomainEventReceived(long position, IEvent @event)
        {
            await _eventPublisher.Publish(@event);
            _lastPosition = position;
        }

        private async Task Disconnected()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            await _eventStore.SubscribeToEventsFrom(_lastPosition, DomainEventReceived, Disconnected);
        }
    }
}