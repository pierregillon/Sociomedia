using System;
using System.Collections.Generic;
using Sociomedia.DomainEvents;

namespace Sociomedia.Domain.Themes
{
    public class NewThemeCreated : DomainEvent
    {
        public NewThemeCreated(Guid id, IReadOnlyCollection<string> keywords, IReadOnlyCollection<ThemeArticle> articles) : base(id, "theme")
        {
            Keywords = keywords;
            Articles = articles;
        }

        public IReadOnlyCollection<string> Keywords { get; }
        public IReadOnlyCollection<ThemeArticle> Articles { get; }
    }
}