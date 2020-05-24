using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Sociomedia.Core.Domain;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public class InMemoryEventStore : IEventStore, IEventStoreExtended
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Dictionary<Guid, List<IEvent>> _domainEventsPerGuid = new Dictionary<Guid, List<IEvent>>();
        private DateTimeOffset _now;

        public InMemoryEventStore(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task<IReadOnlyCollection<IEvent>> GetNewEvents()
        {
            await Task.Delay(0);

            return _domainEventsPerGuid
                .SelectMany(x => x.Value)
                .Where(x => x.TimeStamp > _now)
                .ToArray();
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var @event in events) {
                Add(@event);
                await _eventPublisher.Publish(@event, cancellationToken);
            }
        }

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken())
        {
            await Task.Delay(0, cancellationToken);

            if (_domainEventsPerGuid.TryGetValue(aggregateId, out var events)) {
                return events.Where(x => x.Version > fromVersion).ToArray();
            }
            return Array.Empty<IEvent>();
        }

        private void Add(IEvent @event)
        {
            if (_domainEventsPerGuid.TryGetValue(@event.Id, out var existingEvents)) {
                existingEvents.Add(@event);
            }
            else {
                _domainEventsPerGuid.Add(@event.Id, new List<IEvent> { @event });
            }
        }

        public void CommitEvents()
        {
            _now = DateTimeOffset.Now;
        }

        public IAsyncEnumerable<IEvent> GetAllEventsBetween(Position startPosition, Position endPosition, IReadOnlyCollection<Type> eventTypes)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetCurrentGlobalStreamPosition()
        {
            throw new NotImplementedException();
        }
    }
}