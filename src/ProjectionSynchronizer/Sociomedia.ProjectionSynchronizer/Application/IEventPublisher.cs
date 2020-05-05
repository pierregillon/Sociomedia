using System.Threading.Tasks;

namespace Sociomedia.ProjectionSynchronizer.Application {
    public interface IEventPublisher
    {
        Task Publish(IDomainEvent domainEvent);
    }
}