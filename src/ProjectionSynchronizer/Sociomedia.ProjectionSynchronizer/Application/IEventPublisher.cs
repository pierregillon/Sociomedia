using System.Threading.Tasks;
using Sociomedia.DomainEvents;

namespace Sociomedia.ProjectionSynchronizer.Application {
    public interface IEventPublisher
    {
        Task Publish(DomainEvent domainEvent);
    }
}