using System;

namespace Sociomedia.FeedAggregator.Domain.Rss {
    public class RssSourceSynchronized : DomainEvents.RssSource.RssSourceSynchronized, IDomainEvent
    {
        public RssSourceSynchronized(Guid id, DateTime synchronizedDate) : base(id, synchronizedDate) { }
    }
}