using System;
using FluentAssertions;
using NSubstitute;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure;
using Sociomedia.Themes.Application;
using Sociomedia.Themes.Application.Commands.AddArticleToTheme;
using Sociomedia.Themes.Application.Projections;
using Sociomedia.Themes.Domain;
using Xunit;
using Article = Sociomedia.Themes.Domain.Article;
using Keyword = Sociomedia.Themes.Domain.Keyword;

namespace Sociomedia.Tests
{
    public class ThemeManagerTests
    {
        private readonly ThemeProjection _projection;
        private readonly ThemeManager _themeManager;
        private readonly TimeSpan _twoWeeks = TimeSpan.FromDays(15);

        public ThemeManagerTests()
        {
            var clock = Substitute.For<IClock>();
            clock.Now().Returns(DateTimeOffset.Now);
            _projection = new ThemeProjection(new InMemoryDatabase());
            _themeManager = new ThemeManager(new ThemeDataFinder(_projection, _twoWeeks, clock, new AcceptanceTests.AcceptanceTests.EmptyLogger()));
        }

        [Fact]
        public void A_single_article_does_not_create_theme()
        {
            var article = new Article(Guid.NewGuid(), new[] { new Keyword("test 1", 2), });

            var commands = _themeManager.Add(article);

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

            var newArticle = new Article(article3, new[] {
                new Keyword("coronavirus", 3),
                new Keyword("france", 3),
            });

            var commands = _themeManager.Add(newArticle);

            commands
                .Should()
                .BeEquivalentTo(new[] {
                    new AddArticleToThemeCommand(theme1, newArticle)
                });
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