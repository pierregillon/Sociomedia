using System.Collections.Generic;

namespace NewsAggregator.Domain.Articles
{
    public class Article
    {
        public IReadOnlyCollection<Keyword> Keywords { get; }
        public string Name { get; }
        public string RssFeedId { get; set; }

        public Article(string name, IReadOnlyCollection<Keyword> keywords)
        {
            Name = name;
            Keywords = keywords;
        }
    }
}