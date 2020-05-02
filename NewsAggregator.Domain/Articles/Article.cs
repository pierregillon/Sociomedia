using System;
using System.Collections.Generic;
using CQRSlite.Domain;

namespace NewsAggregator.Domain.Articles
{
    public class Article : AggregateRoot
    {
        public Article(string name, Uri url, Guid rssSourceId, IReadOnlyCollection<Keyword> keywords)
        {
            this.Id = Guid.NewGuid();

            ApplyChange(new ArticleCreated(this.Id, name, url, keywords, rssSourceId));
        }
    }
}