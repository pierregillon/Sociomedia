using System;
using System.Collections.Generic;

namespace Sociomedia.Themes.Domain
{
    public class ThemeAdded : ThemeEvent
    {
        public IReadOnlyCollection<Keyword2> Keywords { get; }
        public IReadOnlyCollection<Guid> Articles { get; }

        public ThemeAdded(Guid id, IReadOnlyCollection<Keyword2> keywords, IReadOnlyCollection<Guid> articles) : base(id)
        {
            Id = id;
            Keywords = keywords;
            Articles = articles;
        }
    }
}