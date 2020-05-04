using System.Threading.Tasks;

namespace NewsAggregator.ReadDatabaseSynchronizer {
    public interface IEventPublisher
    {
        Task Publish(IDomainEvent domainEvent);
    }
}