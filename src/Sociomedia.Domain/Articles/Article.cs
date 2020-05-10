using System;
using System.Collections.Generic;
using CQRSlite.Domain;

namespace Sociomedia.Domain.Articles
{
    public class Article : AggregateRoot
    {
        private string _imageUrl;
        private Article() { }

        public Article(Guid mediaId, ExternalArticle externalArticle, IReadOnlyCollection<string> keywords) : this()
        {
            ApplyChange(new ArticleImported(
                Guid.NewGuid(),
                externalArticle.Title,
                externalArticle.Summary,
                externalArticle.PublishDate,
                externalArticle.Url,
                externalArticle.ImageUrl,
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
                externalArticle.Url,
                externalArticle.ImageUrl ?? _imageUrl
            ));
        }

        private void Apply(ArticleImported @event)
        {
            Id = @event.Id;
            _imageUrl = @event.ImageUrl;
        }
    }
}