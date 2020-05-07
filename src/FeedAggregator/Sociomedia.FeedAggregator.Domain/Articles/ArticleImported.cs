using System;
using System.Collections.Generic;
using CQRSlite.Events;

namespace Sociomedia.Domain.Articles
{
    public class ArticleImported : DomainEvents.Article.ArticleImported, IEvent
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