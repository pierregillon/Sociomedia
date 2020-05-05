using System.Diagnostics;
using System.Threading.Tasks;
using Sociomedia.FeedAggregator.Application;
using Sociomedia.FeedAggregator.Infrastructure.CQRS;

namespace Sociomedia.FeedAggregator.Infrastructure.Logging {
    public class CommandDispatchedLogger : ICommandDispatcher
    {
        private readonly ILogger _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public CommandDispatchedLogger(ILogger logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        public async Task Dispatch<T>(T command) where T : ICommand
        {
            var watch = Stopwatch.StartNew();
            await _logger.LogInformation("START " + command.GetType().Name);
            try
            {
                await _commandDispatcher.Dispatch(command);
            }
            finally {
                await _logger.LogInformation("END " + command.GetType().Name, watch.ElapsedMilliseconds);
            }
        }
    }
}