using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.Domain.Themes
{
    public class ThemeArticle
    {
        public IReadOnlyCollection<string> Keywords { get; }

        public ThemeArticle(IReadOnlyCollection<string> keywords)
        {
            Keywords = keywords;
        }

        public IReadOnlyCollection<string> ContainsKeywords(ThemeArticle article)
        {
            return Keywords.Intersect(article.Keywords).ToArray();
        }

        public bool ContainsKeywords(IReadOnlyCollection<string> keywords)
        {
            return keywords.All(Keywords.Contains);
        }
    }
}