using System;
using System.Collections.Generic;

namespace Sociomedia.FeedAggregator.Domain.Articles
{
    public class ArticleImported : DomainEvents.Article.ArticleImported, IDomainEvent
    {
        public ArticleImported(
            Guid id, 
            string title, 
            string summary, 
            DateTimeOffset publishDate, 
            Uri url, 
            Uri imageUrl, 
            IReadOnlyCollection<string> keywords, 
            Guid mediaId) : base(id, title, summary, publishDate, url.AbsoluteUri, imageUrl?.AbsoluteUri, keywords, mediaId) { }
    }
}