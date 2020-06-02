using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;

namespace Sociomedia.Core.Infrastructure.EventStoring {
    public class EventStorePublisherDecorator : IEventStore
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;

        public EventStorePublisherDecorator(IEventStore eventStore, IEventPublisher eventPublisher)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = new CancellationToken())
        {
            var enumeratedEvents = events.ToArray();
            await _eventStore.Save(enumeratedEvents, cancellationToken);
            foreach (var @event in enumeratedEvents) {
                await _eventPublisher.Publish(@event, cancellationToken);
            }
        }

        public Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken())
        {
            return _eventStore.Get(aggregateId, fromVersion, cancellationToken);
        }
    }
}