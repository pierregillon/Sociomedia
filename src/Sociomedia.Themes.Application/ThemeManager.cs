using System.Collections.Generic;
using Sociomedia.Core.Domain;
using Sociomedia.Themes.Application.Projections;

namespace Sociomedia.Themes.Application
{
    public class ThemeManager
    {
        private readonly List<ArticleReadModel> _articles = new List<ArticleReadModel>();
        private readonly List<ThemeReadModel> _themes = new List<ThemeReadModel>();
        private readonly List<DomainEvent> _uncommittedEvents = new List<DomainEvent>();

        public IReadOnlyCollection<DomainEvent> UncommittedEvents => _uncommittedEvents;

        //public void Add(Article article)
        //{
        //    foreach (var existingArticle in _articles) {
        //        var keywordIntersection = existingArticle.ContainsKeywords(article);
        //        if (!keywordIntersection.Any()) {
        //            continue;
        //        }
        //        var matchingThemes = _themes.Where(theme => theme.Keywords.Intersect(keywordIntersection).Count() == theme.Keywords.Count).ToList();
        //        if (matchingThemes.Any()) {
        //            foreach (var matchingTheme in matchingThemes.Where(matchingTheme => !matchingTheme.Contains(article))) {
        //                Apply(new ArticleAddedToTheme(matchingTheme.Id, article));
        //            }
        //        }
        //        if (matchingThemes.All(x => x.Keywords.Count != keywordIntersection.Count)) {
        //            CreateTheme(keywordIntersection, article);
        //        }
        //    }

        //    foreach (var theme in _themes.ToList()) {
        //        var keywordIntersection = theme.ContainsKeywords(article);
        //        if (!keywordIntersection.Any()) {
        //            continue;
        //        }
        //        var existingTheme = _themes.FirstOrDefault(x => x.Keywords.SequenceEqual(keywordIntersection));
        //        if (existingTheme == null) {
        //            Apply(new NewThemeCreated(Guid.NewGuid(), keywordIntersection, theme.Articles.Append(article).ToArray()));
        //        }
        //        else if (!existingTheme.Contains(article)) {
        //            Apply(new ArticleAddedToTheme(existingTheme.Id, article));
        //        }
        //    }

        //    _articles.Add(article);
        //}

        //private void CreateTheme(IReadOnlyCollection<Keyword> keywordIntersection, Article article)
        //{
        //    var matchingArticles = _articles
        //        .Where(x => x.ContainsKeywords(keywordIntersection))
        //        .Append(article)
        //        .ToArray();

        //    Apply(new NewThemeCreated(Guid.NewGuid(), keywordIntersection, matchingArticles));
        //}

        //private void Apply<T>(T domainEvent) where T : DomainEvent
        //{
        //    ((dynamic) this).On(domainEvent);
        //    _uncommittedEvents.Add(domainEvent);
        //}

        //private void On(NewThemeCreated themeCreated)
        //{
        //    _themes.Add(new Theme(themeCreated.Id, themeCreated.Keywords, themeCreated.Articles));
        //}

        //private void On(ArticleAddedToTheme @event)
        //{
        //    var theme = _themes.Single(x => x.Id == @event.Id);
        //    theme.AddArticle(@event.Article);
        //}
    }
}