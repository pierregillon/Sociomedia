using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator
{
    public class Theme
    {
        private readonly HashSet<Article> articles;

        public Theme(IReadOnlyCollection<Keyword> keywords, IReadOnlyCollection<Article> articles)
        {
            this.Keywords = keywords;
            this.articles = articles.ToHashSet();
        }

        public IReadOnlyCollection<Keyword> Keywords { get; }

        public IReadOnlyCollection<Article> Articles => articles;

        public void AddArticle(Article article)
        {
            articles.Add(article);
        }
    }
}