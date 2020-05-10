using System.Threading.Tasks;
using Sociomedia.Domain;

namespace Sociomedia.ProjectionSynchronizer.Application {
    public interface IEventPublisher
    {
        Task Publish(DomainEvent domainEvent);
    }
}