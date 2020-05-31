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
            return ChallengeExistingThemes(article).Union(ChallengeExistingArticles(article)).Distinct();
        }

        private IEnumerable<ThemeEvent> ChallengeExistingThemes(Article article)
        {
            var keywordIntersectedThemes = _themeProjection.Themes
                .Select(x => new { Theme = x, KeywordIntersection = x.CommonKeywords(article) })
                .Where(x => x.KeywordIntersection.Any())
                .ToArray();

            foreach (var theme in keywordIntersectedThemes) {
                var existingTheme = _themeProjection.Themes.FirstOrDefault(x => theme.KeywordIntersection.SequenceEquals(x.Keywords));
                if (existingTheme == null) {
                    yield return AddTheme(theme.KeywordIntersection.ToArray(), theme.Theme.Articles.Select(x => x.Id).Append(article.Id).ToArray());
                }
                else if (!existingTheme.Contains(article)) {
                    yield return AddArticleToTheme(existingTheme, article);
                }
            }
        }

        private IEnumerable<ThemeEvent> ChallengeExistingArticles(Article article)
        {
            var keywordIntersectedArticles = _themeProjection.Articles
                .Where(x => x.Id != article.Id)
                .Select(x => new { Article = x, KeywordIntersection = x.CommonKeywords(article) })
                .Where(x => x.KeywordIntersection.Any())
                .GroupBy(x => x.KeywordIntersection)
                .ToArray();

            foreach (var value in keywordIntersectedArticles) {
                var matchingThemes = _themeProjection.Themes.Where(theme => value.Key.ContainsAllWords(theme.Keywords)).ToList();
                if (!matchingThemes.Any() || matchingThemes.All(x => x.Keywords.Count != value.Key.Count)) {
                    yield return AddTheme(value.Key.ToArray(), value.Select(x => x.Article.Id).Append(article.Id).ToArray());
                }
            }
        }

        private ThemeAdded AddTheme(IReadOnlyCollection<Keyword2> keywords, IReadOnlyCollection<Guid> articles)
        {
            var @event = new ThemeAdded(Guid.NewGuid(), keywords, articles);
            //_themeProjection.AddTheme(@event);
            return @event;
        }

        private ArticleAddedToTheme AddArticleToTheme(ThemeReadModel theme, Article article)
        {
            var @event = new ArticleAddedToTheme(theme.Id, article.Id);
            //_themeProjection.AddArticleToTheme(@event);
            return @event;
        }
    }
}