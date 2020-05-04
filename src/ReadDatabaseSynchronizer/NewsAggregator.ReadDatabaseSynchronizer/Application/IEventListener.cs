using System.Threading.Tasks;

namespace NewsAggregator.ReadDatabaseSynchronizer.Application
{
    public interface IEventListener<T> where T : IDomainEvent
    {
        Task On(T @event);
    }
}