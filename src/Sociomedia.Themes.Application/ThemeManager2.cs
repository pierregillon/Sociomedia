using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Themes.Application.Commands.AddArticleToTheme;
using Sociomedia.Themes.Application.Projections;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application
{
    public class ThemeManager2
    {
        private readonly ThemeProjection _themeProjection;
        private readonly List<AddArticleToThemeCommand> _commands = new List<AddArticleToThemeCommand>();


        public ThemeManager2(ThemeProjection themeProjection)
        {
            _themeProjection = themeProjection;
        }

        public IEnumerable<ThemeEvent> Add(Article article)
        {
            foreach (var theme in _themeProjection.Themes.ToList()) {
                var keywordIntersection = theme.CommonKeywords(article);
                if (!keywordIntersection.Any()) {
                    continue;
                }
                var existingTheme = _themeProjection.Themes.FirstOrDefault(x => keywordIntersection.SequenceEquals(x.Keywords));
                if (existingTheme == null) {
                    yield return AddTheme(keywordIntersection.ToArray(), theme.Articles.Select(x=>x.Id).Append(article.Id).ToArray());
                }
                else if (!existingTheme.Contains(article)) {
                    yield return AddArticleToTheme(existingTheme, article);
                }
            }

            foreach (var existingArticle in _themeProjection.Articles.Where(x => x.Id != article.Id)) {
                var keywordIntersection1 = existingArticle.CommonKeywords(article);
                if (!keywordIntersection1.Any()) {
                    continue;
                }
                var matchingThemes = _themeProjection.Themes.Where(theme => keywordIntersection1.ContainsAllWords(theme.Keywords)).ToList();
                if (matchingThemes.Any()) {
                    foreach (var matchingTheme in matchingThemes.Where(matchingTheme => !matchingTheme.Contains(article))) {
                        yield return AddArticleToTheme(matchingTheme, article);
                    }
                }
                if (!matchingThemes.Any() || matchingThemes.All(x => x.Keywords.Count != keywordIntersection1.Count)) {
                    yield return AddTheme(keywordIntersection1.ToArray(), new[] { article.Id, existingArticle.Id });
                }
            }
        }

        private ThemeAdded AddTheme(IReadOnlyCollection<Keyword2> keywords, IReadOnlyCollection<Guid> articles)
        {
            var @event = new ThemeAdded(Guid.NewGuid(), keywords, articles);
            _themeProjection.AddTheme(@event);
            return @event;
        }

        private ArticleAddedToTheme AddArticleToTheme(ThemeReadModel theme, Article article)
        {
            var @event = new ArticleAddedToTheme(theme.Id, article.Id);
            _themeProjection.AddArticleToTheme(@event);
            return @event;
        }
    }
}