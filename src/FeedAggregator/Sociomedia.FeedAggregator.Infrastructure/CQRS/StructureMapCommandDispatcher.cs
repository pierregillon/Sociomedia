using System.Threading.Tasks;
using Lamar;
using Sociomedia.FeedAggregator.Application;

namespace Sociomedia.FeedAggregator.Infrastructure.CQRS
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