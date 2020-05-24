using System.Threading;
using System.Threading.Tasks;
using Sociomedia.Articles.Application.Commands.SynchronizeMediaFeeds;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Core.Infrastructure.EventStoring;

namespace Sociomedia.FeedAggregator.Application
{
    public class FeedsSynchronizer
    {
        private readonly IEventBus _eventBus;
        private readonly Configuration _configuration;
        private readonly ICommandDispatcher _commandDispatcher;

        public FeedsSynchronizer(
            IEventBus eventBus,
            Configuration configuration,
            ICommandDispatcher commandDispatcher)
        {
            _eventBus = eventBus;
            _configuration = configuration;
            _commandDispatcher = commandDispatcher;
        }

        public async Task PeriodicallySynchronizeFeeds(CancellationToken token)
        {
            try {
                do {
                    if (_eventBus.IsConnected) {
                        await _commandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());
                    }
                    await Task.Delay(_configuration.FeedAggregator.SynchronizationTimespan, token);
                } while (true);
            }
            catch (TaskCanceledException) { }
        }
    }
}