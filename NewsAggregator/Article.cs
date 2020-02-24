using System.Collections.Generic;

namespace NewsAggregator
{
    public class Article
    {
        public IReadOnlyCollection<Keyword> Keywords { get; }
        public string Name { get; }

        public Article(string name, IReadOnlyCollection<Keyword> keywords)
        {
            Name = name;
            Keywords = keywords;
        }
    }
}