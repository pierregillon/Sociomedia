using System;
using FluentAssertions;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Infrastructure;
using Sociomedia.Themes.Application;
using Sociomedia.Themes.Application.Commands.AddArticleToTheme;
using Sociomedia.Themes.Application.Projections;
using Sociomedia.Themes.Domain;
using Xunit;

namespace Sociomedia.Tests
{
    public class ThemeManagerTests
    {
        private readonly ThemeProjection _projection;
        private readonly ThemeChallenger _themeChallenger;
        private readonly TimeSpan _twoWeeks = TimeSpan.FromDays(15);

        public ThemeManagerTests()
        {
            _projection = new ThemeProjection(new InMemoryDatabase());
            _themeChallenger = new ThemeChallenger(new ThemeDataFinder(_projection, _twoWeeks, new AcceptanceTests.AcceptanceTests.EmptyLogger()));
        }

        [Fact]
        public void A_single_article_does_not_create_theme()
        {
            var article = new ArticleToChallenge(Guid.NewGuid(), DateTimeOffset.Now, new[] { new Keyword("test 1", 2), });

            var commands = _themeChallenger.Challenge(article);

            commands.Should().BeEmpty();
        }

        [Fact]
        public void Adding_article_in_an_existing_theme_that_match_keywords()
        {
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();
            var article3 = Guid.NewGuid();
            var theme1 = Guid.NewGuid();

            _projection.On(AnArticleImported(article1));
            _projection.On(new ArticleKeywordsDefined(article1, new[] {
                new Articles.Domain.Keywords.Keyword("coronavirus", 2),
                new Articles.Domain.Keywords.Keyword("italie", 2),
            }));
            _projection.On(AnArticleImported(article2));
            _projection.On(new ArticleKeywordsDefined(article2, new[] {
                new Articles.Domain.Keywords.Keyword("coronavirus", 3),
                new Articles.Domain.Keywords.Keyword("china", 3),
            }));
            _projection.On(new ThemeAdded(theme1, new[] {
                new Keyword("coronavirus", 5)
            }, new[] { article1, article2 }));

            var articleToChallenge = new ArticleToChallenge(article3, DateTimeOffset.Now, new[] {
                new Keyword("coronavirus", 3),
                new Keyword("france", 3),
            });

            var commands = _themeChallenger.Challenge(articleToChallenge);

            commands
                .Should()
                .BeEquivalentTo(new AddArticleToThemeCommand(theme1, articleToChallenge.ToDomain()));
        }

        private static ArticleImported AnArticleImported(Guid articleId, DateTimeOffset publishDate = default)
        {
            return new ArticleImported(
                articleId,
                "some title",
                "some summary",
                publishDate == default ? DateTimeOffset.Now : publishDate,
                "some url",
                "some image url",
                "some external id",
                Array.Empty<string>(),
                Guid.NewGuid()
            );
        }
    }
}