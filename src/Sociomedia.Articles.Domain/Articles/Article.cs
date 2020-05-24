using System;
using System.Collections.Generic;
using CQRSlite.Domain;
using Sociomedia.Articles.Domain.Feeds;
using Sociomedia.Articles.Domain.Keywords;

namespace Sociomedia.Articles.Domain.Articles
{
    public class Article : AggregateRoot
    {
        private string _imageUrl;
        private Article() { }

        public Article(Guid mediaId, FeedItem feedItem) : this()
        {
            ApplyChange(new ArticleImported(
                Guid.NewGuid(),
                feedItem.Title,
                feedItem.Summary,
                feedItem.PublishDate,
                feedItem.Link,
                feedItem.ImageUrl,
                feedItem.Id,
                Array.Empty<string>(),
                mediaId));
        }

        public string Url { get; private set; }
        public string Title { get; private set; }
        public string Summary { get; private set; }

        public void DefineKeywords(IReadOnlyCollection<Keyword> keywords)
        {
            ApplyChange(new ArticleKeywordsDefined(Id, keywords));
        }

        public void UpdateFromFeed(FeedItem feedItem)
        {
            ApplyChange(new ArticleUpdated(
                Id,
                feedItem.Title,
                feedItem.Summary,
                feedItem.PublishDate,
                feedItem.Link,
                feedItem.ImageUrl ?? _imageUrl
            ));
        }

        public void Delete()
        {
            ApplyChange(new ArticleDeleted(Id));
        }

        private void Apply(ArticleImported @event)
        {
            Id = @event.Id;
            Url = @event.Url;
            Title = @event.Title;
            Summary = @event.Summary;

            _imageUrl = @event.ImageUrl;
        }
    }
}