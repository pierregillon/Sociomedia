using System;
using System.Collections.Generic;
using CQRSlite.Domain;

namespace NewsAggregator.Domain.Articles
{
    public class Article : AggregateRoot
    {
        private Article() { }

        public Article(string title, Uri url, Guid rssSourceId, IReadOnlyCollection<Keyword> keywords) : this()
        {
            ApplyChange(new ArticleCreated(Guid.NewGuid(), title, url, keywords, rssSourceId));
        }

        private void Apply(ArticleCreated @event)
        {
            Id = @event.Id;
        }
    }
}