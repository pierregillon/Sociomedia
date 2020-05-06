using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using Lamar;
using Sociomedia.FeedAggregator.Application;

namespace Sociomedia.FeedAggregator.Infrastructure.CQRS
{
    public class StructureMapEventPublisher : IEventPublisher
    {
        private readonly IContainer _container;

        public StructureMapEventPublisher(IContainer container)
        {
            _container = container;
        }

        public async Task Publish<T>(T @event, CancellationToken cancellationToken = new CancellationToken()) where T : class, IEvent
        {
            foreach (var listener in _container.GetAllInstances(typeof(IEventListener<>).MakeGenericType(@event.GetType()))) {
                await On(listener, @event);
            }
        }

        private static async Task On(object listener, IEvent @event)
        {
            await (Task) listener
                .GetType()
                .GetMethods()
                .Where(x => x.Name == "On")
                .Single(x => x.GetParameters()[0].ParameterType == @event.GetType())
                .Invoke(listener, new[] { @event });
        }
    }
}