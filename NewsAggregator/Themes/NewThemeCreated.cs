using System;
using System.Collections.Generic;

namespace NewsAggregator.Themes
{
    public class NewThemeCreated : IDomainEvent
    {
        public IReadOnlyCollection<string> Keywords { get; }
        public IReadOnlyCollection<ThemeArticle> Articles { get; }
        public Guid Id { get;  }

        public NewThemeCreated(Guid id, IReadOnlyCollection<string> keywords, IReadOnlyCollection<ThemeArticle> articles)
        {
            Keywords = keywords;
            Articles = articles;
            Id = id;
        }
    }
}