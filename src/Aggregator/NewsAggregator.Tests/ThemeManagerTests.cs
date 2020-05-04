using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using NewsAggregator.Domain.Themes;
using Xunit;

namespace NewsAggregator.Tests
{
    public class ThemeManagerTests
    {
        private readonly ThemeManager _themeManager;

        public ThemeManagerTests()
        {
            _themeManager = new ThemeManager();
        }

        [Fact]
        public void A_single_article_does_not_create_theme()
        {
            var article = new ThemeArticle(new[] { "test 1" });

            _themeManager.Add(article);

            _themeManager.UncommittedEvents
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void Two_articles_with_same_keywords_create_theme_with_keyword_intersection()
        {
            var article1 = new ThemeArticle(new[] { "coronavirus", "italie" });
            var article2 = new ThemeArticle(new[] { "coronavirus", "china" });

            _themeManager.Add(article1);
            _themeManager.Add(article2);

            _themeManager.UncommittedEvents
                .Should()
                .BeEquivalentTo(new {
                    Keywords = new[] { "coronavirus" },
                    Articles = new[] { article1, article2 }
                });
        }

        [Fact]
        public void Three_articles_create_two_themes()
        {
            var article1 = new ThemeArticle(new[] { "coronavirus", "italie" });
            var article2 = new ThemeArticle(new[] { "coronavirus", "china" });
            var article3 = new ThemeArticle(new[] { "opera", "china" });

            _themeManager.Add(article1);
            _themeManager.Add(article2);
            _themeManager.Add(article3);

            _themeManager.UncommittedEvents
                .Should()
                .BeEquivalentTo(new[] {
                    new {
                        Keywords = new[] { "coronavirus" },
                        Articles = new[] { article1, article2 }
                    },
                    new {
                        Keywords = new[] { "china" },
                        Articles = new[] { article2, article3 }
                    }
                });
        }

        [Fact]
        public void Three_articles_in_the_same_theme()
        {
            var article1 = new ThemeArticle(new[] { "coronavirus", "italie" });
            var article2 = new ThemeArticle(new[] { "coronavirus", "china" });
            var article3 = new ThemeArticle(new[] { "coronavirus", "europe" });

            _themeManager.Add(article1);
            _themeManager.Add(article2);
            _themeManager.Add(article3);

            var events = _themeManager.UncommittedEvents;

            events
                .OfType<NewThemeCreated>()
                .Should()
                .BeEquivalentTo(new {
                    Keywords = new[] { "coronavirus" },
                    Articles = new[] { article1, article2 }
                });

            events
                .OfType<ArticleAddedToTheme>()
                .Should()
                .BeEquivalentTo(new { Article = article3 });
        }

        [Fact]
        public void Three_articles_in_two_themes_with_keyword_in_common()
        {
            var article1 = new ThemeArticle(new[] { "coronavirus", "italie" });
            var article2 = new ThemeArticle(new[] { "coronavirus", "china" });
            var article3 = new ThemeArticle(new[] { "coronavirus", "china" });

            _themeManager.Add(article1);
            _themeManager.Add(article2);
            _themeManager.Add(article3);

            var events = _themeManager.UncommittedEvents;

            events
                .OfType<NewThemeCreated>()
                .Should()
                .BeEquivalentTo(new[] {
                    new {
                        Keywords = new[] { "coronavirus" },
                        Articles = new[] { article1, article2 }
                    },
                    new {
                        Keywords = new[] { "coronavirus", "china" },
                        Articles = new[] { article2, article3 }
                    }
                });

            events
                .OfType<ArticleAddedToTheme>()
                .Should()
                .BeEquivalentTo(new { Article = article3 });
        }

        [Fact]
        public void Three_articles_with_a_single_keyword_in_common()
        {
            var article1 = new ThemeArticle(new[] { "coronavirus", "italie" });
            var article2 = new ThemeArticle(new[] { "coronavirus", "italie", "chine" });
            var article3 = new ThemeArticle(new[] { "coronavirus", "chine" });

            _themeManager.Add(article1);
            _themeManager.Add(article2);
            _themeManager.Add(article3);

            var events = _themeManager.UncommittedEvents;

            events
                .OfType<NewThemeCreated>()
                .Should()
                .BeEquivalentTo(new[] {
                    new {
                        Keywords = new[] { "coronavirus", "italie" },
                        Articles = new[] { article1, article2 }
                    },
                    new {
                        Keywords = new[] { "coronavirus" },
                        Articles = new[] { article1, article2, article3 }
                    },
                    new {
                        Keywords = new[] { "coronavirus", "chine" },
                        Articles = new[] { article2, article3 }
                    },
                });
        }
    }
}