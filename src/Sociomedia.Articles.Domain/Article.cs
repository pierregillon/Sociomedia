using System;
using System.Collections.Generic;
using CQRSlite.Domain;
using CQRSlite.Events;

namespace Sociomedia.Articles.Domain
{
    public class Article : AggregateRoot
    {
        private string _imageUrl;
        private Article() { }

        public Article(Guid mediaId, ExternalArticle externalArticle) : this()
        {
            ApplyChange(new ArticleImported(
                Guid.NewGuid(),
                externalArticle.Title,
                externalArticle.Summary,
                externalArticle.PublishDate,
                externalArticle.Url,
                externalArticle.ImageUrl,
                externalArticle.Id,
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