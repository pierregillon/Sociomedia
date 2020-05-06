using System;
using System.Collections.Generic;
using CQRSlite.Domain;
using Sociomedia.FeedAggregator.Domain.Medias;

namespace Sociomedia.FeedAggregator.Domain.Articles
{
    public class Article : AggregateRoot
    {
        private Article() { }

        public Article(Guid mediaId, ExternalArticle externalArticle, IReadOnlyCollection<string> keywords) : this()
        {
            ApplyChange(new ArticleSynchronized(
                Guid.NewGuid(), 
                externalArticle.Title, 
                externalArticle.Summary,
                externalArticle.PublishDate,
                externalArticle.Url,
                externalArticle.ImageUrl,
                keywords, 
                mediaId));
        }

        private void Apply(ArticleSynchronized @event)
        {
            Id = @event.Id;
        }
    }
}