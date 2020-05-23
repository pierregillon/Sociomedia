using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure.CQRS;

namespace Sociomedia.Core.Infrastructure.Logging
{
    public class CommandDispatcherLogger : ICommandDispatcher
    {
        private readonly ILogger _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public CommandDispatcherLogger(ILogger logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        public async Task Dispatch<T>(T command) where T : ICommand
        {
            Log(command);
            await _commandDispatcher.Dispatch(command);
        }

        public async Task<TResult> Dispatch<T, TResult>(T command) where T : ICommand<TResult>
        {
            Log(command);
            return await _commandDispatcher.Dispatch<T, TResult>(command);
        }

        private void Log(object command)
        {
            var json = JsonConvert.SerializeObject(command, Formatting.None);
            _logger.Info($"[COMMAND_DISPATCHER] Execute command {command.GetType().Name} | {json}");
        }
    }
}