using System;

namespace Sociomedia.DomainEvents.RssSource {
    public class RssSourceSynchronized : DomainEvent
    {
        public RssSourceSynchronized(Guid id, DateTime synchronizedDate) : base(id)
        {
            Id = id;
            SynchronizedDate = synchronizedDate;
        }

        public DateTime SynchronizedDate { get; }
    }
}