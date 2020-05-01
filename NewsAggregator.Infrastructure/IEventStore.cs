using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewsAggregator.Domain;

namespace NewsAggregator.Infrastructure
{
    public interface IEventStore2
    {
        Task<IReadOnlyCollection<IDomainEvent>> GetAllEvents();
        Task<EventHistory> GetEvents(Guid aggregateId);
        Task StoreEvents(IReadOnlyCollection<IDomainEvent> events);
    }
}