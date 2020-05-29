using System;
using System.Collections.Generic;
using Sociomedia.Core.Domain;

namespace Sociomedia.Themes.Domain
{
    public class NewThemeCreated : DomainEvent
    {
        public NewThemeCreated(Guid id, IReadOnlyCollection<string> keywords, IReadOnlyCollection<Article> articles) : base(id, "theme")
        {
            Keywords = keywords;
            Articles = articles;
        }

        public IReadOnlyCollection<string> Keywords { get; }
        public IReadOnlyCollection<Article> Articles { get; }
    }
}