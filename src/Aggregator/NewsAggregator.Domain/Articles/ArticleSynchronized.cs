using System;
using System.Collections.Generic;

namespace NewsAggregator.Domain.Articles
{
    public class ArticleSynchronized : DomainEvent
    {
        public ArticleSynchronized(Guid id, string title, Uri url, IReadOnlyCollection<Keyword> keywords, Guid rssSourceId)
        {
            Id = id;
            Title = title;
            Url = url;
            Keywords = keywords;
            RssSourceId = rssSourceId;
        }

        public string Title { get; }
        public IReadOnlyCollection<Keyword> Keywords { get; }
        public Uri Url { get; }
        public Guid RssSourceId { get; }
    }
}