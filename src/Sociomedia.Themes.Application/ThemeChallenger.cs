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
            foreach (var group in GetKeywordIntersectedGroup(article, _themeDataFinder.GetAllThemes())) {
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
            foreach (var group in GetKeywordIntersectedGroup(article, _themeDataFinder.GetAllArticles(article.Id))) {
                var matchingThemes = _themeDataFinder.GetThemesWithAllKeywordsIncluded(group.KeywordIntersection);
                if (!matchingThemes.Any()) {
                    yield return new CreateNewThemeCommand(group.KeywordIntersection, group.GetAllArticles());
                }
            }
        }

        private static IEnumerable<KeywordIntersectedGroup> GetKeywordIntersectedGroup(ArticleToChallenge article, IEnumerable<ICanIntersectKeywords> elements)
        {
            return elements
                .Select(x => new KeywordIntersected(x, x.IntersectKeywords(article)))
                .Where(x => x.KeywordIntersection.IsCompatibleForThemeCreation())
                .GroupBy(x => x.KeywordIntersection)
                .Select(x => new KeywordIntersectedGroup(x.Key, article, x));
        }

        // ----- Internal classes

        public class KeywordIntersectedGroup
        {
            private readonly ArticleToChallenge _article;
            private readonly IEnumerable<KeywordIntersected> _keywordIntersectedArticles;

            public Keywords KeywordIntersection { get; }

            public KeywordIntersectedGroup(Keywords keywordIntersection, ArticleToChallenge article, IEnumerable<KeywordIntersected> keywordIntersectedArticles)
            {
                KeywordIntersection = keywordIntersection;
                _article = article;
                _keywordIntersectedArticles = keywordIntersectedArticles;
            }

            public IReadOnlyCollection<Article> GetAllArticles()
            {
                return _keywordIntersectedArticles
                    .SelectMany(x => x.CanIntersectKeywords.GetArticles())
                    .Distinct()
                    .Append(_article.ToDomain())
                    .ToArray();
            }
        }

        public class KeywordIntersected
        {
            public ICanIntersectKeywords CanIntersectKeywords { get; }
            public Keywords KeywordIntersection { get; }

            public KeywordIntersected(ICanIntersectKeywords canIntersectKeywords, Keywords keywordIntersection)
            {
                CanIntersectKeywords = canIntersectKeywords;
                KeywordIntersection = keywordIntersection;
            }
        }
    }
}