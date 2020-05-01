using System.Collections.Generic;
using System.Threading.Tasks;
using NewsAggregator.Domain;

namespace NewsAggregator.Infrastructure.CQRS
{
    public interface IEventPublisher2
    {
        Task Publish(IReadOnlyCollection<IDomainEvent> events);
    }
}