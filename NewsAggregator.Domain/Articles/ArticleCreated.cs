using System;
using System.Collections.Generic;
using NewsAggregator.Domain.Themes;

namespace NewsAggregator.Domain.Articles {
    public class ArticleCreated : IDomainEvent
    {
        public Guid Id { get; }
        public string Name { get; }
        public IReadOnlyCollection<Keyword> Keywords { get; }

        public ArticleCreated(Guid id, string name, IReadOnlyCollection<Keyword> keywords)
        {
            Id = id;
            Name = name;
            Keywords = keywords;
        }
    }
}