using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace NewsAggregator.Tests
{
    public class ThemeBuilderTests
    {
        private readonly ThemeBuilder themeBuilder;

        public ThemeBuilderTests()
        {
            this.themeBuilder = new ThemeBuilder();
        }

        [Fact]
        public void A_single_article_does_not_create_theme()
        {
            Article[] articles = new[] {
                new Article("test 1", new []{
                    new Keyword("cononavirus", 10)
                })
            };

            IEnumerable<Theme> themes = this.themeBuilder.Build(articles);

            themes.Should().BeEmpty();
        }

        [Fact]
        public void Two_articles_with_same_keywords_create_theme_with_keyword_intersection()
        {
            Article[] articles = new[] {
                new Article("1", new []{
                    new Keyword("cononavirus", 10),
                    new Keyword("italie", 4)
                }),
                new Article("2", new []{
                    new Keyword("cononavirus", 10),
                    new Keyword("china", 2)
                })
            };

            IEnumerable<Theme> themes = this.themeBuilder.Build(articles);

            themes.Should().BeEquivalentTo(new Theme(new[] { new Keyword("cononavirus", 0) }, articles));
        }

        [Fact]
        public void Three_articles_create_two_themes()
        {
            Article article1 = new Article("1", new[]{
                new Keyword("cononavirus", 10),
                new Keyword("italie", 4)
            });
            Article article2 = new Article("2", new[]{
                new Keyword("cononavirus", 10),
                new Keyword("china", 2)
            });
            Article article3 = new Article("3", new[]{
                new Keyword("opera", 10),
                new Keyword("china", 2)
            });

            IEnumerable<Theme> themes = this.themeBuilder.Build(new[] { article1, article2, article3 });

            themes.Should().BeEquivalentTo(new[] {
                new Theme(new[] { new Keyword("cononavirus", 0) }, new []{ article1, article2 }),
                new Theme(new[] { new Keyword("china", 0) }, new []{ article2, article3 })
            });
        }

        [Fact]
        public void Three_articles_in_the_same_theme()
        {
            Article article1 = new Article("1", new[]{
                new Keyword("cononavirus", 10),
                new Keyword("italie", 4)
            });
            Article article2 = new Article("2", new[]{
                new Keyword("cononavirus", 10),
                new Keyword("china", 2)
            });
            Article article3 = new Article("3", new[]{
                new Keyword("cononavirus", 10),
                new Keyword("europe", 2)
            });

            IEnumerable<Theme> themes = this.themeBuilder.Build(new[] { article1, article2, article3 });

            themes.Should().BeEquivalentTo(new[] {
                new Theme(new[] { new Keyword("cononavirus", 0) }, new []{ article1, article2, article3 }),
            });
        }

        [Fact]
        public void Three_articles_in_two_themes_with_keyword_in_common()
        {
            Article article1 = new Article("1", new[]{
                new Keyword("cononavirus", 10),
                new Keyword("italie", 4)
            });
            Article article2 = new Article("2", new[]{
                new Keyword("cononavirus", 10),
                new Keyword("china", 2)
            });
            Article article3 = new Article("3", new[]{
                new Keyword("cononavirus", 10),
                new Keyword("china", 2)
            });

            IEnumerable<Theme> themes = this.themeBuilder.Build(new[] { article1, article2, article3 });

            themes.Should().BeEquivalentTo(new[] {
                new Theme(new[] { new Keyword("cononavirus", 0) }, new []{ article1, article2, article3 }),
                new Theme(new[] { new Keyword("cononavirus", 0), new Keyword("china", 0) }, new []{ article2, article3 }),
            });
        }
    }
}
