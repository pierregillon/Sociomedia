using System.Collections.Generic;
using System.Threading.Tasks;
using CQRSlite.Events;

namespace Sociomedia.Core.Infrastructure.CQRS
{
    public static class EventPublisherExtensions
    {
        public static async Task Publish(this IEventPublisher eventPublisher, IEnumerable<IEvent> events)
        {
            foreach (var @event in events) {
                await eventPublisher.Publish(@event);
            }
        }
    }
}