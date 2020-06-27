using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Projections
{
    public class ThemeReadModel : ICanIntersectKeywords
    {
        private readonly HashSet<ArticleReadModel> _articles;

        public ThemeReadModel(Guid id, IReadOnlyCollection<string> keywords, IReadOnlyCollection<ArticleReadModel> articles)
        {
            Id = id;
            Keywords = new Keywords(keywords);

            if (articles.Select(x => x.Id).Distinct().Count() != articles.Count) {
                throw new Exception("conflict");
            }

            _articles = articles.ToHashSet();
        }

        public Guid Id { get; }

        public Keywords Keywords { get; }

        public IReadOnlyCollection<ArticleReadModel> Articles => _articles;

        public void AddArticle(ArticleReadModel article)
        {
            if (_articles.Any(x => x.Id == article.Id)) {
                throw new Exception("conflict");
            }
            _articles.Add(article);
        }

        public bool Contains(ArticleToChallenge article)
        {
            return _articles.Select(x => x.Id).Contains(article.Id);
        }

        public Keywords IntersectKeywords(ArticleToChallenge article)
        {
            return Keywords.Intersect(article.Keywords);
        }

        public IEnumerable<Article> GetArticles()
        {
            return this.Articles.Select(x => x.ToDomain());
        }

        public override string ToString()
        {
            return string.Join(" | ", Keywords);
        }

        public ThemeReadModel FilterRecentArticlesFrom(DateTimeOffset date)
        {
            return new ThemeReadModel(Id, Keywords.ToValues(), Articles.Where(x => x.PublishDate > date).ToArray());
        }
    }
}