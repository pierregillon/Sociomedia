using System;

namespace NewsAggregator.Domain.Rss {
    public class RssSourceAdded : DomainEvent
    {
        public RssSourceAdded(Guid aggregateId, string url)
        {
            Id = aggregateId;
            Url = url;
        }

        public string Url { get; }
    }
}