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
                externalArticle.Id,
                keywords,
                mediaId));
        }

        public void Update(ExternalArticle externalArticle)
        {
            ApplyChange(new ArticleUpdated(
                Id,
                externalArticle.Title,
                externalArticle.Summary,
                externalArticle.PublishDate,
                externalArticle.Url.AbsoluteUri,
                externalArticle.ImageUrl?.AbsoluteUri
            ));
        }

        private void Apply(ArticleImported @event)
        {
            Id = @event.Id;
        }
    }
}