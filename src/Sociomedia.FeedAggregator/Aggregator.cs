using System;
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

        public async Task StartAggregation()
        {
            await InitializeEventStore();

            do {
                await Task.Delay(_configuration.FeedAggregator.SynchronizationTimespan);
                await _commandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());
            } while (true);
        }

        private async Task InitializeEventStore()
        {
            await _eventStore.Connect(
                _configuration.EventStore.Server,
                _configuration.EventStore.Port,
                _configuration.EventStore.Login,
                _configuration.EventStore.Password
            );

            await _eventStore.StartRepublishingEvents(_lastPosition, DomainEventReceived, Disconnected);
        }

        private async Task DomainEventReceived(long position, IEvent @event)
        {
            await _eventPublisher.Publish(@event);
            _lastPosition = position;
        }

        private async Task Disconnected()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            await InitializeEventStore();
        }
    }
}