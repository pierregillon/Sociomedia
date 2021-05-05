using System;
using System.Threading.Tasks;
using Sociomedia.Core.Application;
using StructureMap;

namespace Sociomedia.Core.Infrastructure.CQRS
{
    public class StructureMapCommandDispatcher : ICommandDispatcher
    {
        private readonly IContainer _container;

        public StructureMapCommandDispatcher(IContainer container)
        {
            _container = container;
        }

        public async Task DispatchGeneric(ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            var handler = _container.GetInstance(handlerType);

            await (Task)handlerType
                .GetMethod(nameof(ICommandHandler<ICommand>.Handle))
                .Invoke(handler, new[] { command });
        }

        public async Task Dispatch<T>(T command) where T : ICommand
        {
            await _container.GetInstance<ICommandHandler<T>>().Handle(command);
        }

        public async Task<TResult> Dispatch<T, TResult>(T command) where T : ICommand<TResult>
        {
            return await _container.GetInstance<ICommandHandler<T, TResult>>().Handle(command);
        }
    }
}