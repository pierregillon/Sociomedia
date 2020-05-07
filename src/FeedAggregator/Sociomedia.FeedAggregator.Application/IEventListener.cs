using System.Threading.Tasks;
using Sociomedia.DomainEvents;

namespace Sociomedia.FeedAggregator.Application
{
    public interface IEventListener<in T> where T : DomainEvent
    {
        Task On(T @event);
    }
}