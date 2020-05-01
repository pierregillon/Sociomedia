using System.Threading.Tasks;
using NewsAggregator.Domain;

namespace NewsAggregator.Application
{
    public interface IEventListener<in T> where T : IDomainEvent
    {
        Task On(T @event);
    }
}