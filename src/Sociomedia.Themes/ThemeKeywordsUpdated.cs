using System;
using System.Collections.Generic;

namespace Sociomedia.Themes.Domain
{
    public class ThemeKeywordsUpdated : ThemeEvent
    {
        public IReadOnlyCollection<Keyword> Keywords { get; }

        public ThemeKeywordsUpdated(Guid id, IReadOnlyCollection<Keyword> keywords) : base(id)
        {
            Keywords = keywords;
        }
    }
}