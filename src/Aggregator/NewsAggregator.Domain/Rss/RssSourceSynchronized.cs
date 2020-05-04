using System;

namespace NewsAggregator.Domain.Rss {
    public class RssSourceSynchronized : DomainEvent
    {
        public RssSourceSynchronized(Guid id, DateTime synchronizedDate)
        {
            Id = id;
            SynchronizedDate = synchronizedDate;
        }

        public DateTime SynchronizedDate { get; }
    }
}