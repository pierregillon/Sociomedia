using System;
using System.Collections.Generic;

namespace NewsAggregator.Domain.Themes
{
    public class NewThemeCreated : DomainEvent
    {
        public NewThemeCreated(Guid id, IReadOnlyCollection<string> keywords, IReadOnlyCollection<ThemeArticle> articles)
        {
            Keywords = keywords;
            Articles = articles;
            Id = id;
        }

        public IReadOnlyCollection<string> Keywords { get; }
        public IReadOnlyCollection<ThemeArticle> Articles { get; }
    }
}