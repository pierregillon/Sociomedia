using System;
using System.Collections.Generic;
using System.Linq;
using CQRSlite.Domain;
using Sociomedia.Core.Domain;

namespace Sociomedia.Themes.Domain
{
    public class Theme : AggregateRoot
    {
        private IReadOnlyCollection<Keyword2> _keywords;

        private Theme() { }

        public Theme(IReadOnlyCollection<Article> articles)
        {
            var keywords = IntersectKeywords(articles.Select(x => x.Keywords).ToArray());

            ApplyChange(new ThemeAdded(Guid.NewGuid(), keywords, articles.Select(x => x.Id).ToArray()));
        }

        public void AddArticle(Article article)
        {
            ApplyChange(new ArticleAddedToTheme(Id, article.Id));
            ApplyChange(new ThemeKeywordsUpdated(Id, IntersectKeywords(new[] { _keywords, article.Keywords })));
        }

        private void Apply(ThemeAdded @event)
        {
            Id = @event.Id;
            _keywords = @event.Keywords;
        }

        private void Apply(ThemeKeywordsUpdated @event)
        {
            _keywords = @event.Keywords;
        }

        private static IReadOnlyCollection<Keyword2> IntersectKeywords(IReadOnlyCollection<IEnumerable<Keyword2>> keywordsList)
        {
            var keywordValues = keywordsList.Select(x => x.Select(a => a.Value)).IntersectAll();

            return keywordsList
                .SelectMany(x => x)
                .Where(x => keywordValues.Contains(x.Value))
                .GroupBy(x => x.Value)
                .Select(g => g.Aggregate((x, y) => x + y))
                .ToArray();
        }
    }
}