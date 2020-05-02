using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using NewsAggregator.Domain;

namespace NewsAggregator.Infrastructure
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Dictionary<Guid, List<IEvent>> _domainEventsPerGuid = new Dictionary<Guid, List<IEvent>>();

        public InMemoryEventStore(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task<IReadOnlyCollection<IEvent>> GetAllEvents()
        {
            await Task.Delay(0);

            return _domainEventsPerGuid.SelectMany(x => x.Value).ToArray();
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = new CancellationToken())
        {
            await Task.Delay(0, cancellationToken);

            var fixedEvents = events.ToArray();

            foreach (var @event in fixedEvents) {
                if (_domainEventsPerGuid.TryGetValue(@event.Id, out var existingEvents)) {
                    existingEvents.Add(@event);
                }
                else {
                    _domainEventsPerGuid.Add(@event.Id, new List<IEvent> { @event });
                }
            }

            foreach (var @event in fixedEvents) {
                await _eventPublisher.Publish(@event, cancellationToken);
            }
        }

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken())
        {
            await Task.Delay(0, cancellationToken);

            if (_domainEventsPerGuid.TryGetValue(aggregateId, out var events)) {
                return events.Where(x => x.Version > fromVersion).ToArray();
            }
            return EventHistory.Empty();
        }
    }
}