using System.Threading.Tasks;
using Sociomedia.FeedAggregator.Domain;

namespace Sociomedia.FeedAggregator.Application
{
    public interface IEventListener<in T> where T : IDomainEvent
    {
        Task On(T @event);
    }
}