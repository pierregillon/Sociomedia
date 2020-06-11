using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
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
        private readonly ThemeProjection _themeProjection;
        private readonly ILogger _logger;


        public ThemeManager(ThemeProjection themeProjection, ILogger logger)
        {
            _themeProjection = themeProjection;
            _logger = logger;
        }

        public IEnumerable<ICommand> Add(Article article)
        {
            return ChallengeExistingThemes(article).Union(ChallengeExistingArticles(article)).Distinct();
        }

        private IEnumerable<ICommand> ChallengeExistingThemes(Article article)
        {
            var keywordIntersectedThemes = _themeProjection.Themes
                .Select(x => new { Theme = x, KeywordIntersection = x.CommonKeywords(article) })
                .Where(x => x.KeywordIntersection.Any())
                .GroupBy(x => x.KeywordIntersection)
                .ToArray();

            foreach (var intersection in keywordIntersectedThemes) {
                var existingTheme = FindTheme(intersection.Key);
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
            var keywordIntersectedArticles = _themeProjection.Articles
                .Where(x => x.Id != article.Id)
                .Select(x => new { Article = x, KeywordIntersection = x.CommonKeywords(article) })
                .Where(x => x.KeywordIntersection.Any())
                .GroupBy(x => x.KeywordIntersection)
                .ToArray();

            foreach (var intersection in keywordIntersectedArticles) {
                var matchingThemes = _themeProjection.Themes.Where(theme => intersection.Key.ContainsAllWords(theme.Keywords)).ToList();
                if (!matchingThemes.Any()) {
                    yield return intersection
                        .Select(x => x.Article.ToDomain())
                        .Append(article)
                        .ToArray()
                        .Pipe(x => new CreateNewThemeCommand(intersection.Key, x));
                }
            }
        }

        private ThemeReadModel FindTheme(KeywordIntersection keywordIntersection)
        {
            var themes = _themeProjection.Themes.Where(x => keywordIntersection.SequenceEquals(x.Keywords)).ToArray();
            if (themes.Length > 1) {
                _logger.Info("2 themes have the same keyword intersections !");
            }
            return themes.FirstOrDefault();
        }
    }
}