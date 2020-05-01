using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NewsAggregator.Domain;

namespace NewsAggregator.Infrastructure {
    public class EventHistory : IReadOnlyCollection<IDomainEvent>
    {
        private readonly IReadOnlyCollection<IDomainEvent> _events;

        public EventHistory(IEnumerable<IDomainEvent> @events)
        {
            _events = @events.ToArray();
        }

        public IEnumerator<IDomainEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _events.Count;

        public static EventHistory Empty()
        {
            return new EventHistory(Enumerable.Empty<IDomainEvent>());
        }
    }
}