using System;
using System.Linq;
using FluentAssertions;
using Sociomedia.Core.Domain;
using Sociomedia.Themes.Domain;
using Xunit;

namespace Sociomedia.Tests
{
    public class ThemeTests
    {
        [Fact]
        public void New_theme_calculates_keyword_intersection_ordered_by_occurence_then_by_value()
        {
            var articles = new[] {
                new Article(Guid.NewGuid(), new[] { new Keyword("b", 2), new Keyword("a", 2), new Keyword("c", 3) }),
                new Article(Guid.NewGuid(), new[] { new Keyword("b", 2), new Keyword("a", 2), new Keyword("c", 5) }),
            };

            var theme = new Theme(articles);

            theme
                .GetUncommittedChanges()
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] {
                        new Keyword("c", 8),
                        new Keyword("a", 4),
                        new Keyword("b", 4),
                    }, new[] { articles[0].Id, articles[1].Id })
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public void Adding_an_article_to_a_theme_recalculate_keyword_intersection()
        {
            var articles = new[] {
                new Article(Guid.NewGuid(), new[] { new Keyword("b", 2), new Keyword("a", 2), new Keyword("c", 3) }),
                new Article(Guid.NewGuid(), new[] { new Keyword("b", 2), new Keyword("a", 2), new Keyword("c", 5) }),
            };

            var theme = new Theme(articles);

            var newArticle = new Article(Guid.NewGuid(), new[] { new Keyword("b", 2) });

            theme.AddArticle(newArticle);

            theme
                .GetUncommittedChanges()
                .Skip(1)
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ArticleAddedToTheme(default, newArticle.Id), 
                    new ThemeKeywordsUpdated(default, new[] { new Keyword("b", 6) }), 
                }, x => x.ExcludeDomainEventTechnicalFields());
        }
    }
}