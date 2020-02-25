using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator.Themes
{
    public class Theme
    {
        private readonly HashSet<ThemeArticle> _articles;

        public Theme(Guid id, IReadOnlyCollection<string> keywords, IEnumerable<ThemeArticle> articles)
        {
            Id = id;
            Keywords = keywords;
            _articles = articles.ToHashSet();
        }

        public Guid Id { get; }

        public IReadOnlyCollection<string> Keywords { get; }

        public IReadOnlyCollection<ThemeArticle> Articles => _articles;

        public void AddArticle(ThemeArticle article)
        {
            _articles.Add(article);
        }

        public bool Contains(ThemeArticle article)
        {
            return _articles.Contains(article);
        }

        public IReadOnlyCollection<string> ContainsKeywords(ThemeArticle article)
        {
            return Keywords.Intersect(article.Keywords).ToArray();
        }
    }
}