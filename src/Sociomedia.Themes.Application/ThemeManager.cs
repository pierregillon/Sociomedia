using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using Sociomedia.Core.Application;
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

            foreach (var intersectedTheme in keywordIntersectedThemes) {
                var existingThemes = _themeProjection.Themes.Where(x => intersectedTheme.Key.SequenceEquals(x.Keywords)).ToArray();
                if (existingThemes.Length > 1) {
                    _logger.Info("2 themes have the same keyword intersections !");
                }
                var existingTheme = existingThemes.FirstOrDefault();
                if (existingTheme == null) {
                    yield return AddTheme(intersectedTheme.Key, intersectedTheme.SelectMany(x => x.Theme.Articles).Select(x => x.ToDomain()).Append(article).ToArray());
                }
                else if (!existingTheme.Contains(article)) {
                    yield return AddArticleToTheme(existingTheme, article);
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

            foreach (var value in keywordIntersectedArticles) {
                var matchingThemes = _themeProjection.Themes.Where(theme => value.Key.ContainsAllWords(theme.Keywords)).ToList();
                if (!matchingThemes.Any()) {
                    yield return AddTheme(value.Key, value.Select(x => x.Article.ToDomain()).Append(article).ToArray());
                }
            }
        }

        private static ICommand AddTheme(KeywordIntersection intersection, IReadOnlyCollection<Article> articles)
        {
            //var @event = new ThemeAdded(Guid.NewGuid(), keywords, articles);
            //_themeProjection.AddTheme(@event);
            return new CreateNewThemeCommand(intersection, articles);
        }

        private static ICommand AddArticleToTheme(ThemeReadModel theme, Article article)
        {
            //var @event = new ArticleAddedToTheme(theme.Id, article.Id);
            //_themeProjection.AddArticleToTheme(@event);
            return new AddArticleToThemeCommand(theme.Id, article);
        }
    }
}