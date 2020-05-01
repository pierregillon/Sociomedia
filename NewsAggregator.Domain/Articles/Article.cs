using System;
using System.Collections.Generic;
using NewsAggregator.Domain.Themes;

namespace NewsAggregator.Domain.Articles
{
    public class Article
    {
        public IReadOnlyCollection<Keyword> Keywords { get; }
        public string Name { get; }
        public string RssFeedId { get; set; }
        public List<IDomainEvent> UncommitedEvents { get; } = new List<IDomainEvent>();

        public Article(string name, IReadOnlyCollection<Keyword> keywords)
        {
            Name = name;
            Keywords = keywords;

            UncommitedEvents.Add(new ArticleCreated(Guid.NewGuid(), name, keywords));
        }
    }
}