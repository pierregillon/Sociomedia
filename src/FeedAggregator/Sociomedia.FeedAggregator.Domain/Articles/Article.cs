using System;
using System.Collections.Generic;
using CQRSlite.Domain;

namespace Sociomedia.Domain.Articles
{
    public class Article : AggregateRoot
    {
        private Article() { }

        public Article(Guid mediaId, ExternalArticle externalArticle, IReadOnlyCollection<string> keywords) : this()
        {
            ApplyChange(new ArticleImported(
                Guid.NewGuid(), 
                externalArticle.Title, 
                externalArticle.Summary,
                externalArticle.PublishDate,
                externalArticle.Url.AbsoluteUri,
                externalArticle.ImageUrl?.AbsoluteUri,
                keywords, 
                mediaId));
        }

        private void Apply(ArticleImported @event)
        {
            Id = @event.Id;
        }
    }
}