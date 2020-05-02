using System.Diagnostics;
using System.Threading.Tasks;
using NewsAggregator.Application;
using NewsAggregator.Infrastructure.CQRS;

namespace NewsAggregator.Infrastructure.Logging {
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