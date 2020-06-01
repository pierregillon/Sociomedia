using System;
using System.Collections.Generic;

namespace Sociomedia.Themes.Domain
{
    public class ThemeKeywordsUpdated : ThemeEvent
    {
        public IReadOnlyCollection<Keyword2> Keywords { get; }

        public ThemeKeywordsUpdated(Guid id, IReadOnlyCollection<Keyword2> keywords) : base(id)
        {
            Keywords = keywords;
        }
    }
}