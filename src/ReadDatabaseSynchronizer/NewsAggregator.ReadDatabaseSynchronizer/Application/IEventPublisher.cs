using System.Threading.Tasks;

namespace NewsAggregator.ReadDatabaseSynchronizer.Application {
    public interface IEventPublisher
    {
        Task Publish(IDomainEvent domainEvent);
    }
}