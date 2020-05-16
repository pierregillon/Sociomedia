using System.Threading.Tasks;
using Sociomedia.Application.Domain;

namespace Sociomedia.ProjectionSynchronizer.Application {
    public interface IEventPublisher
    {
        Task Publish(DomainEvent domainEvent);
    }
}