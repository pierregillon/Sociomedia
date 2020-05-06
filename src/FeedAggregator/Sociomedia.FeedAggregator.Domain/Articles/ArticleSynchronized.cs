using System;
using System.Collections.Generic;

namespace Sociomedia.FeedAggregator.Domain.Articles
{
    public class ArticleSynchronized : DomainEvents.Article.ArticleSynchronized, IDomainEvent
    {
        public ArticleSynchronized(Guid id, string title, string summary, DateTimeOffset publishDate, Uri url, Uri imageUrl, IReadOnlyCollection<string> keywords, Guid rssSourceId) : base(id, title, summary, publishDate, url.AbsoluteUri, imageUrl?.AbsoluteUri, keywords, rssSourceId) { }
    }
}