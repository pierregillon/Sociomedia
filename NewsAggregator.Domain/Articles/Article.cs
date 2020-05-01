using System;
using System.Collections.Generic;
using CQRSlite.Domain;

namespace NewsAggregator.Domain.Articles
{
    public class Article : AggregateRoot
    {
        public IReadOnlyCollection<Keyword> Keywords { get; }
        public string Name { get; }
        public string RssFeedId { get; set; }

        public Article(string name, string url, Guid rssSourceId, IReadOnlyCollection<Keyword> keywords)
        {
            Id = Guid.NewGuid();
            Name = name;
            Keywords = keywords;

            ApplyChange(new ArticleCreated(Id, name, url, keywords, rssSourceId));
        }
    }
}