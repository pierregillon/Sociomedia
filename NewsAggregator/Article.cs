using System.Collections.Generic;

namespace NewsAggregator
{
    public class Article {
        public IReadOnlyCollection<Keyword> Keywords { get; }

        public Article(IReadOnlyCollection<Keyword> keywords)
        {
            Keywords = keywords;
        }
    }
}