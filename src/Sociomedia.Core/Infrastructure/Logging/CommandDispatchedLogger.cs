using System.Threading.Tasks;
using EventStore.ClientAPI;
using Sociomedia.Application.Application;
using Sociomedia.Application.Infrastructure.CQRS;

namespace Sociomedia.Application.Infrastructure.Logging
{
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
            try {
                _logger.Debug("STARTING " + command.GetType().Name);
                await _commandDispatcher.Dispatch(command);
            }
            finally {
                _logger.Debug("END " + command.GetType().Name);
            }
        }

        public async Task<TResult> Dispatch<T, TResult>(T command) where T : ICommand<TResult>
        {
            try {
                _logger.Debug("STARTING " + command.GetType().Name);
                return await _commandDispatcher.Dispatch<T, TResult>(command);
            }
            finally {
                _logger.Debug("END " + command.GetType().Name);
            }
        }
    }
}