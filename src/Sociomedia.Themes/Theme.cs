using System;
using System.Collections.Generic;
using System.Linq;
using CQRSlite.Domain;
using Sociomedia.Core.Domain;

namespace Sociomedia.Themes.Domain
{
    public class Theme : AggregateRoot
    {
        private IReadOnlyCollection<Keyword> _keywords;
        private readonly List<Guid> _articles = new List<Guid>();

        private Theme() { }

        public Theme(IReadOnlyCollection<Article> articles)
        {
            var keywords = IntersectKeywords(articles.Select(x => x.Keywords).ToArray());

            ApplyChange(new ThemeAdded(Guid.NewGuid(), keywords, articles.Select(x => x.Id).ToArray()));
        }

        public void AddArticle(Article article)
        {
            if (_articles.Contains(article.Id)) {
                throw new InvalidOperationException($"The article {article.Id} is already present in the theme {this.Id}.");
            }
            ApplyChange(new ArticleAddedToTheme(Id, article.Id));
            ApplyChange(new ThemeKeywordsUpdated(Id, IntersectKeywords(new[] { _keywords, article.Keywords })));
        }

        private void Apply(ThemeAdded @event)
        {
            Id = @event.Id;
            _keywords = @event.Keywords;
            _articles.AddRange(@event.Articles);
        }

        private void Apply(ThemeKeywordsUpdated @event)
        {
            _keywords = @event.Keywords;
        }

        private void Apply(ArticleAddedToTheme @event)
        {
            _articles.Add(@event.ArticleId);
        }

        private static IReadOnlyCollection<Keyword> IntersectKeywords(IReadOnlyCollection<IEnumerable<Keyword>> keywordsList)
        {
            var commonKeywords = keywordsList
                .Select(x => x.Select(a => a.Value))
                .IntersectAll()
                .ToArray();

            var test = keywordsList
                .SelectMany(x => x)
                .Where(x => commonKeywords.Contains(x.Value))
                .GroupBy(x => x.Value)
                .Select(g => g.Aggregate((x, y) => x + y))
                .OrderByDescending(x => x.Occurence)
                .ThenBy(x => x.Value)
                .ToArray();

            if (test.Length > 5) {
                throw new InvalidOperationException("something wrong");
            }

            return test;
        }
    }
}