using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Application.Commands.AddArticleToTheme;
using Sociomedia.Themes.Application.Commands.CreateNewTheme;
using Sociomedia.Themes.Application.Projections;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application
{
    public class ThemeChallenger
    {
        private readonly ThemeDataFinder _themeDataFinder;

        public ThemeChallenger(ThemeDataFinder themeDataFinder)
        {
            _themeDataFinder = themeDataFinder;
        }

        public IEnumerable<ICommand> Challenge(ArticleToChallenge article)
        {
            return ChallengeExistingThemes(article).Union(ChallengeExistingArticles(article)).Distinct();
        }

        // ----- Private

        private IEnumerable<ICommand> ChallengeExistingThemes(ArticleToChallenge article)
        {
            foreach (var group in GetKeywordIntersectedThemeGroups(article)) {
                var existingTheme = _themeDataFinder.FindExistingTheme(group.KeywordIntersection);
                if (existingTheme == null) {
                    yield return new CreateNewThemeCommand(group.KeywordIntersection, group.GetAllArticles());
                }
                else if (!existingTheme.Contains(article)) {
                    yield return new AddArticleToThemeCommand(existingTheme.Id, article.ToDomain());
                }
            }
        }

        private IEnumerable<ICommand> ChallengeExistingArticles(ArticleToChallenge article)
        {
            foreach (var group in GetKeywordIntersectedArticlesGroup(article)) {
                var matchingThemes = _themeDataFinder.GetThemesWithAllKeywordsIncluded(group.KeywordIntersection, article);
                if (!matchingThemes.Any()) {
                    yield return new CreateNewThemeCommand(group.KeywordIntersection, group.GetAllArticles());
                }
            }
        }

        private IEnumerable<KeywordIntersectedArticleGroup> GetKeywordIntersectedArticlesGroup(ArticleToChallenge article)
        {
            return _themeDataFinder
                .GetArticlesInSameTimeFrame(article)
                .Select(x => new KeywordIntersectedArticle(x, x.IntersectKeywords(article)))
                .Where(x => x.KeywordIntersection.Any())
                .GroupBy(x => x.KeywordIntersection)
                .Select(x => new KeywordIntersectedArticleGroup(x.Key, article, x))
                .ToArray();
        }

        private IEnumerable<KeywordIntersectedThemeGroup> GetKeywordIntersectedThemeGroups(ArticleToChallenge article)
        {
            return _themeDataFinder
                .GetThemesContainingArticlesInSameTimeFrame(article)
                .Select(theme => new KeywordIntersectedTheme(theme.IntersectKeywords(article), theme))
                .Where(x => x.KeywordIntersection.Any())
                .GroupBy(x => x.KeywordIntersection)
                .Select(x => new KeywordIntersectedThemeGroup(x.Key, article, x))
                .ToArray();
        }

        // ----- Internal classes

        public class KeywordIntersectedArticleGroup
        {
            private readonly ArticleToChallenge _article;
            private readonly IEnumerable<KeywordIntersectedArticle> _keywordIntersectedArticles;

            public Keywords KeywordIntersection { get; }

            public KeywordIntersectedArticleGroup(Keywords keywordIntersection, ArticleToChallenge article, IEnumerable<KeywordIntersectedArticle> keywordIntersectedArticles)
            {
                KeywordIntersection = keywordIntersection;
                _article = article;
                _keywordIntersectedArticles = keywordIntersectedArticles;
            }

            public IReadOnlyCollection<Article> GetAllArticles()
            {
                return _keywordIntersectedArticles
                    .Select(x => x.Article.ToDomain())
                    .Append(_article.ToDomain())
                    .ToArray();
            }
        }

        public class KeywordIntersectedArticle
        {
            public ArticleReadModel Article { get; }
            public Keywords KeywordIntersection { get; }

            public KeywordIntersectedArticle(ArticleReadModel article, Keywords keywordIntersection)
            {
                Article = article;
                KeywordIntersection = keywordIntersection;
            }
        }

        public class KeywordIntersectedThemeGroup
        {
            private readonly ArticleToChallenge _article;
            private readonly IEnumerable<KeywordIntersectedTheme> _group;

            public Keywords KeywordIntersection { get; }

            public KeywordIntersectedThemeGroup(Keywords keywordIntersection, ArticleToChallenge article, IEnumerable<KeywordIntersectedTheme> keywordIntersectedGroup)
            {
                _article = article;
                KeywordIntersection = keywordIntersection;
                _group = keywordIntersectedGroup;
            }

            public IReadOnlyCollection<Article> GetAllArticles()
            {
                return _group
                    .SelectMany(x => x.Articles)
                    .Select(x => x.ToDomain())
                    .Distinct()
                    .Append(_article.ToDomain())
                    .ToArray();
            }
        }

        public class KeywordIntersectedTheme
        {
            private readonly ThemeReadModel _theme;

            public KeywordIntersectedTheme(Keywords keywordIntersection, ThemeReadModel theme)
            {
                _theme = theme;
                KeywordIntersection = keywordIntersection;
            }

            public Keywords KeywordIntersection { get; }
            public IReadOnlyCollection<ArticleReadModel> Articles => _theme.Articles;
        }
    }
}