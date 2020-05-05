using System.Linq;
using System.Threading.Tasks;
using Sociomedia.ProjectionSynchronizer.Application;
using StructureMap;

namespace Sociomedia.ProjectionSynchronizer.Infrastructure
{
    public class StructureMapEventPublisher : IEventPublisher
    {
        private readonly IContainer _container;

        public StructureMapEventPublisher(IContainer container)
        {
            _container = container;
        }

        public async Task Publish(IDomainEvent @event)
        {
            foreach (var listener in _container.GetAllInstances(typeof(IEventListener<>).MakeGenericType(@event.GetType()))) {
                await On(listener, @event);
            }
        }

        private static async Task On(object listener, IDomainEvent domainEvent)
        {
            await (Task) listener
                .GetType()
                .GetMethods()
                .Where(x => x.Name == nameof(IEventListener<IDomainEvent>.On))
                .Single(x => x.GetParameters()[0].ParameterType == domainEvent.GetType())
                .Invoke(listener, new[] { domainEvent });
        }
    }
}