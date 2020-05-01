using System.Collections.Generic;
using System.Threading.Tasks;
using NewsAggregator.Application;
using NewsAggregator.Domain;
using StructureMap;

namespace NewsAggregator.Infrastructure.CQRS
{
    public class StructureMapEventPublisher : IEventPublisher
    {
        private readonly IContainer _container;

        public StructureMapEventPublisher(IContainer container)
        {
            _container = container;
        }

        public async Task Publish(IReadOnlyCollection<IDomainEvent> events)
        {
            foreach (var @event in events) {
                foreach (var listener in _container.GetAllInstances(typeof(IEventListener<>).MakeGenericType(@event.GetType()))) {
                    await On(listener, @event);
                }
            }
        }

        private static async Task On(object listener, IDomainEvent @event)
        {
            await (Task) listener
                .GetType()
                .GetMethod("On")
                .MakeGenericMethod(@event.GetType())
                .Invoke(listener, new[] { @event });
        }
    }
}