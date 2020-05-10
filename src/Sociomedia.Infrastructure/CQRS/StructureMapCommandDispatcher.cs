using System.Threading.Tasks;
using Sociomedia.Application;
using StructureMap;

namespace Sociomedia.Infrastructure.CQRS
{
    public class StructureMapCommandDispatcher : ICommandDispatcher
    {
        private readonly IContainer _container;

        public StructureMapCommandDispatcher(IContainer container)
        {
            _container = container;
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