using System;
using System.Collections.Generic;
using CQRSlite.Domain;

namespace NewsAggregator.Domain.Articles
{
    public class Article : AggregateRoot
    {
        private Article() { }

        public Article(string title, Uri url, Guid rssSourceId, IReadOnlyCollection<string> keywords) : this()
        {
            ApplyChange(new ArticleSynchronized(Guid.NewGuid(), title, url, keywords, rssSourceId));
        }

        private void Apply(ArticleSynchronized @event)
        {
            Id = @event.Id;
        }
    }
}