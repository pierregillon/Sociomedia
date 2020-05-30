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

        public ThemeReadModel(Guid id, IReadOnlyCollection<Keyword2> keywords, IEnumerable<ArticleReadModel> articles)
        {
            Id = id;
            Keywords = keywords;
            _articles = articles.ToHashSet();
        }

        public Guid Id { get; }

        public IReadOnlyCollection<Keyword2> Keywords { get; }

        public IReadOnlyCollection<ArticleReadModel> Articles => _articles;

        public void AddArticle(ArticleReadModel article)
        {
            _articles.Add(article);
        }

        public bool Contains(ArticleReadModel article)
        {
            return _articles.Select(x => x.Id).Contains(article.Id);
        }

        public bool Contains(Article article)
        {
            return _articles.Select(x => x.Id).Contains(article.Id);
        }

        public Keywords2 CommonKeywords(Article article)
        {
            return Keywords
                .Join(article.Keywords, x => x.Value, y => y.Value, (x, y) => x + y)
                .ToArray()
                .Pipe(x => new Keywords2(x));
        }
    }
}