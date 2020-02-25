using System;
using System.Collections.Generic;
using System.Text;

namespace NewsAggregator.Themes
{
    public class ThemeArticle
    {
        public IReadOnlyCollection<string> Keywords { get; }

        public ThemeArticle(IReadOnlyCollection<string> keywords)
        {
            Keywords = keywords;
        }
    }
}
