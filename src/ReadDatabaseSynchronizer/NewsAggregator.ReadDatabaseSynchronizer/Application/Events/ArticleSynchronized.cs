using System;
using System.Collections.Generic;

namespace NewsAggregator.ReadDatabaseSynchronizer.Application.Events
{
    public class ArticleSynchronized : IDomainEvent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public IReadOnlyCollection<string> Keywords { get; set; }
        public Uri Url { get; set; }
    }
}