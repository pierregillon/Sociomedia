using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Application.Commands.AddArticleToTheme;
using Sociomedia.Themes.Application.Commands.CreateNewTheme;
using Sociomedia.Themes.Application.Projections;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application
{
    public class ThemeManager2
    {
        private readonly ThemeProjection _themeProjection;


        public ThemeManager2(ThemeProjection themeProjection)
        {
            _themeProjection = themeProjection;
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
                .ToArray();

            foreach (var theme in keywordIntersectedThemes) {
                var existingTheme = _themeProjection.Themes.FirstOrDefault(x => theme.KeywordIntersection.SequenceEquals(x.Keywords));
                if (existingTheme == null) {
                    yield return AddTheme(theme.Theme.Articles.Select(x => x.ToDomain()).Append(article).ToArray());
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
                if (!matchingThemes.Any() || matchingThemes.All(x => x.Keywords.Count != value.Key.Count)) {
                    yield return AddTheme(value.Select(x => x.Article.ToDomain()).Append(article).ToArray());
                }
            }
        }

        private static ICommand AddTheme(IReadOnlyCollection<Article> articles)
        {
            //var @event = new ThemeAdded(Guid.NewGuid(), keywords, articles);
            //_themeProjection.AddTheme(@event);
            return new CreateNewThemeCommand(articles);
        }

        private static ICommand AddArticleToTheme(ThemeReadModel theme, Article article)
        {
            //var @event = new ArticleAddedToTheme(theme.Id, article.Id);
            //_themeProjection.AddArticleToTheme(@event);
            return new AddArticleToThemeCommand(theme.Id, article);
        }
    }
}