using System.Threading.Tasks;
using NewsAggregator.Application;
using StructureMap;

namespace NewsAggregator.Infrastructure.CQRS
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
    }
}