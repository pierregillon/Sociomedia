using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Application;
using Sociomedia.Core.Domain;
using Sociomedia.Themes.Application.Commands.AddArticleToTheme;
using Sociomedia.Themes.Application.Commands.CreateNewTheme;
using Sociomedia.Themes.Application.Projections;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application
{
    public class ThemeManager
    {
        private readonly ThemeDataFinder _themeDataFinder;

        public ThemeManager(ThemeDataFinder themeDataFinder)
        {
            _themeDataFinder = themeDataFinder;
        }

        public IEnumerable<ICommand> Add(Article article)
        {
            return ChallengeExistingThemes(article).Union(ChallengeExistingArticles(article)).Distinct();
        }

        private IEnumerable<ICommand> ChallengeExistingThemes(Article article)
        {
            var keywordIntersectedThemes = _themeDataFinder.GetThemesWithRecentlyArticleAdded()
                .Select(x => new { Theme = x, KeywordIntersection = x.CommonKeywords(article) })
                .Where(x => x.KeywordIntersection.Any())
                .GroupBy(x => x.KeywordIntersection)
                .ToArray();

            foreach (var intersection in keywordIntersectedThemes) {
                var existingTheme = _themeDataFinder.FindTheme(intersection.Key);
                if (existingTheme == null) {
                    yield return intersection
                        .SelectMany(x => x.Theme.Articles)
                        .Select(x => x.ToDomain())
                        .Distinct()
                        .Append(article)
                        .ToArray()
                        .Pipe(x => new CreateNewThemeCommand(intersection.Key, x));
                }
                else if (!existingTheme.Contains(article)) {
                    yield return new AddArticleToThemeCommand(existingTheme.Id, article);
                }
            }
        }

        private IEnumerable<ICommand> ChallengeExistingArticles(Article article)
        {
            var keywordIntersectedArticles = _themeDataFinder.GetRecentArticles()
                .Where(x => x.Id != article.Id)
                .Select(x => new { Article = x, KeywordIntersection = x.CommonKeywords(article) })
                .Where(x => x.KeywordIntersection.Any())
                .GroupBy(x => x.KeywordIntersection)
                .ToArray();

            foreach (var intersection in keywordIntersectedArticles) {
                var matchingThemes = _themeDataFinder.GetAllKeywordIncludedThemes(intersection.Key);
                if (!matchingThemes.Any()) {
                    yield return intersection
                        .Select(x => x.Article.ToDomain())
                        .Append(article)
                        .ToArray()
                        .Pipe(x => new CreateNewThemeCommand(intersection.Key, x));
                }
            }
        }
    }
}