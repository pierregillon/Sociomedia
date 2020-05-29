using System;
using FluentAssertions;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Domain.Keywords;
using Sociomedia.Themes.Application;
using Sociomedia.Themes.Application.Commands.CreateNewTheme;
using Sociomedia.Themes.Domain;
using Xunit;
using Article = Sociomedia.Themes.Domain.Article;

namespace Sociomedia.Tests
{
    public class ThemeManagerTests
    {
        private ThemeProjection _projection;
        private ThemeManager2 _themeManager2;

        public ThemeManagerTests()
        {
            _projection = new ThemeProjection();
            _themeManager2 = new ThemeManager2(_projection);
        }

        [Fact]
        public void A_single_article_does_not_create_theme()
        {
            var themeManager2 = new ThemeManager2(new ThemeProjection());

            var article = new Article(Guid.NewGuid(), new[] { new Keyword2("test 1", 2), });

            var commands = themeManager2.Add(article);

            commands.Should().BeEmpty();
        }

        [Fact]
        public void Two_articles_with_same_keywords_create_theme_with_keyword_intersection()
        {
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();

            _projection.AddArticle(new ArticleKeywordsDefined(article1, new[] {
                new Keyword("coronavirus", 2),
                new Keyword("italie", 2),
            }));

            var newArticle = new Article(article2, new[] {
                new Keyword2("coronavirus", 3),
                new Keyword2("china", 3),
            });

            var commands = _themeManager2.Add(newArticle);

            commands
                .Should()
                .BeEquivalentTo(new[] {
                    new CreateNewThemeCommand(
                        new[] { new Keyword2("coronavirus", 5) },
                        new[] { article1, article2 }
                    )
                });
        }

        [Fact]
        public void Three_articles_create_two_themes()
        {
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();
            var article3 = Guid.NewGuid();

            _projection.AddArticle(new ArticleKeywordsDefined(article1, new[] {
                new Keyword("coronavirus", 2),
                new Keyword("italie", 2),
            }));
            _projection.AddArticle(new ArticleKeywordsDefined(article2, new[] {
                new Keyword("opera", 2),
                new Keyword("china", 2),
            }));

            var newArticle = new Article(article3, new[] {
                new Keyword2("coronavirus", 3),
                new Keyword2("china", 3),
            });

            var commands = _themeManager2.Add(newArticle);

            commands
                .Should()
                .BeEquivalentTo(new[] {
                    new CreateNewThemeCommand(
                        new[] { new Keyword2("coronavirus", 5) },
                        new[] { article1, article3 }
                    ),
                    new CreateNewThemeCommand(
                        new[] { new Keyword2("china", 5) },
                        new[] { article2, article3 }
                    )
                });
        }

        [Fact]
        public void Adding_article_in_an_existing_theme_that_match_keywords()
        {
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();
            var article3 = Guid.NewGuid();
            var theme1 = Guid.NewGuid();

            _projection.AddArticle(new ArticleKeywordsDefined(article1, new[] {
                new Keyword("coronavirus", 2),
                new Keyword("italie", 2),
            }));
            _projection.AddArticle(new ArticleKeywordsDefined(article2, new[] {
                new Keyword("coronavirus", 3),
                new Keyword("china", 3),
            }));
            _projection.AddTheme(new ThemeAdded(theme1, new[] {
                new Keyword2("coronavirus", 5)
            }, new[] { article1, article2 }));

            var newArticle = new Article(article3, new[] {
                new Keyword2("coronavirus", 3),
                new Keyword2("france", 3),
            });

            var commands = _themeManager2.Add(newArticle);

            commands
                .Should()
                .BeEquivalentTo(new[] {
                    new AddArticleToThemeCommand(theme1, newArticle)
                });
        }

        //[Fact]
        //public void Three_articles_in_the_same_theme()
        //{
        //    var article1 = new Article(new[] { "coronavirus", "italie" });
        //    var article2 = new Article(new[] { "coronavirus", "china" });
        //    var article3 = new Article(new[] { "coronavirus", "europe" });

        //    _themeManager.Add(article1);
        //    _themeManager.Add(article2);
        //    _themeManager.Add(article3);

        //    var events = _themeManager.UncommittedEvents;

        //    events
        //        .OfType<NewThemeCreated>()
        //        .Should()
        //        .BeEquivalentTo(new {
        //            Keywords = new[] { "coronavirus" },
        //            Articles = new[] { article1, article2 }
        //        });

        //    events
        //        .OfType<ArticleAddedToTheme>()
        //        .Should()
        //        .BeEquivalentTo(new { Article = article3 });
        //}

        //[Fact]
        //public void Three_articles_in_two_themes_with_keyword_in_common()
        //{
        //    var article1 = new Article(new[] { "coronavirus", "italie" });
        //    var article2 = new Article(new[] { "coronavirus", "china" });
        //    var article3 = new Article(new[] { "coronavirus", "china" });

        //    _themeManager.Add(article1);
        //    _themeManager.Add(article2);
        //    _themeManager.Add(article3);

        //    var events = _themeManager.UncommittedEvents;

        //    events
        //        .OfType<NewThemeCreated>()
        //        .Should()
        //        .BeEquivalentTo(new[] {
        //            new {
        //                Keywords = new[] { "coronavirus" },
        //                Articles = new[] { article1, article2 }
        //            },
        //            new {
        //                Keywords = new[] { "coronavirus", "china" },
        //                Articles = new[] { article2, article3 }
        //            }
        //        });

        //    events
        //        .OfType<ArticleAddedToTheme>()
        //        .Should()
        //        .BeEquivalentTo(new { Article = article3 });
        //}

        //[Fact]
        //public void Three_articles_with_a_single_keyword_in_common()
        //{
        //    var article1 = new Article(new[] { "coronavirus", "italie" });
        //    var article2 = new Article(new[] { "coronavirus", "italie", "chine" });
        //    var article3 = new Article(new[] { "coronavirus", "chine" });

        //    _themeManager.Add(article1);
        //    _themeManager.Add(article2);
        //    _themeManager.Add(article3);

        //    var events = _themeManager.UncommittedEvents;

        //    events
        //        .OfType<NewThemeCreated>()
        //        .Should()
        //        .BeEquivalentTo(new[] {
        //            new {
        //                Keywords = new[] { "coronavirus", "italie" },
        //                Articles = new[] { article1, article2 }
        //            },
        //            new {
        //                Keywords = new[] { "coronavirus" },
        //                Articles = new[] { article1, article2, article3 }
        //            },
        //            new {
        //                Keywords = new[] { "coronavirus", "chine" },
        //                Articles = new[] { article2, article3 }
        //            },
        //        });
        //}
    }
}