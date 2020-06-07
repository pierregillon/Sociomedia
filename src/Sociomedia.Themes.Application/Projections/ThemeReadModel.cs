using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Domain;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ThemeReadModel
    {
        private readonly HashSet<ArticleReadModel> _articles;

        public ThemeReadModel(Guid id, IReadOnlyCollection<string> keywords, IEnumerable<ArticleReadModel> articles)
        {
            Id = id;
            Keywords = keywords;
            _articles = articles.ToHashSet();
        }

        public Guid Id { get; }

        public IReadOnlyCollection<string> Keywords { get; }

        public IReadOnlyCollection<ArticleReadModel> Articles => _articles;

        public void AddArticle(ArticleReadModel article)
        {
            if (_articles.Any(x => x.Id == article.Id)) {
                throw new Exception("conflict");
            }
            _articles.Add(article);
        }

        public bool Contains(Article article)
        {
            return _articles.Select(x => x.Id).Contains(article.Id);
        }

        public KeywordIntersection CommonKeywords(Article article)
        {
            return Keywords
                .Intersect(article.Keywords.Select(x => x.Value))
                .ToArray()
                .Pipe(x => new KeywordIntersection(x));
        }

        public override string ToString()
        {
            return string.Join(" | ", Keywords);
        }
    }
}