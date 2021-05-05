using System;
using System.Collections.Generic;

namespace Sociomedia.Front.Data
{
    public class TrendingThemeListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<ArticleListItem> Articles { get; set; }
    }
}