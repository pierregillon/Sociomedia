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
        private readonly List<IEvent> _allEvents = new List<IEvent>();
        private DateTimeOffset _now;
        private bool _republish;

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

        public async Task StoreAndPublish(IEnumerable<IEvent> events, CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var @event in events) {
                Add(@event);
                await _eventPublisher.Publish(@event, cancellationToken);
            }
        }

        public Task Store(IEnumerable<IEvent> events, CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var @event in events) {
                Add(@event);
            }
            return Task.CompletedTask;
        }

        Task IEventStore.Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = new CancellationToken())
        {
            if (_republish) {
                return StoreAndPublish(events, cancellationToken);
            }
            else {
                return Store(events, cancellationToken);
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

            _allEvents.Add(@event);
        }

        public void CommitEvents()
        {
            _now = DateTimeOffset.Now;
        }

        public async IAsyncEnumerable<IEvent> GetAllEventsBetween(Position startPosition, Position endPosition, IReadOnlyCollection<Type> eventTypes)
        {
            await Task.Delay(0);

            var events = _allEvents
                .Skip((int) startPosition.CommitPosition)
                .Where((x, i) => i < endPosition.CommitPosition)
                .ToArray();

            foreach (var @event in events) {
                yield return @event;
            }
        }

        public Task<long> GetCurrentGlobalStreamPosition()
        {
            throw new NotImplementedException();
        }

        public void EnableRepublish()
        {
            _republish = true;
        }
    }
}