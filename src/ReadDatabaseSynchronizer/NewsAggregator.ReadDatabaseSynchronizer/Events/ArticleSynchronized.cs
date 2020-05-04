using System;
using System.Collections.Generic;

namespace NewsAggregator.ReadDatabaseSynchronizer.Events
{
    public class ArticleSynchronized : IDomainEvent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public IReadOnlyCollection<Keyword> Keywords { get; set; }
        public Uri Url { get; set; }
        public Guid RssSourceId { get; set; }
    }
}