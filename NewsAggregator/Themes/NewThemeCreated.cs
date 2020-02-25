using System;
using System.Collections.Generic;

namespace NewsAggregator.Themes
{
    public class NewThemeCreated : IDomainEvent
    {
        public IReadOnlyCollection<string> Keywords { get; }
        public IEnumerable<ThemeArticle> Articles { get; }
        public Guid Id { get;  }

        public NewThemeCreated(Guid id, IReadOnlyCollection<string> keywords, IEnumerable<ThemeArticle> articles)
        {
            Keywords = keywords;
            Articles = articles;
            Id = id;
        }
    }
}