using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Domain.Keywords;
using Sociomedia.Core.Domain;
using Sociomedia.Themes.Application.Projections;
using Sociomedia.Themes.Domain;
using Xunit;

namespace Sociomedia.Tests.AcceptanceTests
{
    public class ChallengeThemesOnArticleEventReceived : AcceptanceTests
    {
        [Fact]
        public async Task Create_new_theme_on_article_keywords_defined_with_common_keywords()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();

            await EventStore.StoreAndPublish(new[] {
                new ArticleKeywordsDefined(article1Id, new[] { new Keyword("coronavirus", 2), new Keyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { new Keyword("coronavirus", 3), new Keyword("chine", 2) }),
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword2("coronavirus", 5) }, new[] { article1Id, article2Id })
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Add_article_to_existing_theme()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();
            var article3Id = Guid.NewGuid();

            await EventStore.StoreAndPublish(new[] {
                new ArticleKeywordsDefined(article1Id, new[] { new Keyword("coronavirus", 2), new Keyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { new Keyword("coronavirus", 3), new Keyword("chine", 2) }),
                new ArticleKeywordsDefined(article3Id, new[] { new Keyword("coronavirus", 3), new Keyword("italie", 3) }),
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword2("coronavirus", 5) }, new[] { article1Id, article2Id }),
                    new ArticleAddedToTheme(default, article3Id)
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Create_2_themes_on_article_keywords_defined()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();
            var article3Id = Guid.NewGuid();

            await EventStore.StoreAndPublish(new[] {
                new ArticleKeywordsDefined(article1Id, new[] { new Keyword("coronavirus", 2), new Keyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { new Keyword("opera", 3), new Keyword("chine", 2) }),
                new ArticleKeywordsDefined(article3Id, new[] { new Keyword("coronavirus", 5), new Keyword("chine", 3) }),
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword2("coronavirus", 7) }, new[] { article1Id, article3Id }),
                    new ThemeAdded(default, new[] { new Keyword2("chine", 5) }, new[] { article2Id, article3Id }),
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Create_a_new_theme_on_keyword_theme_intersection()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();
            var article3Id = Guid.NewGuid();

            await EventStore.StoreAndPublish(new[] {
                new ArticleKeywordsDefined(article1Id, new[] { new Keyword("coronavirus", 2), new Keyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { new Keyword("coronavirus", 3), new Keyword("france", 3) }),
                new ArticleKeywordsDefined(article3Id, new[] { new Keyword("coronavirus", 5), new Keyword("chine", 3) }),
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword2("coronavirus", 5), new Keyword2("france", 5) }, new[] { article1Id, article2Id }),
                    new ThemeAdded(default, new[] { new Keyword2("coronavirus", 10) }, new[] { article1Id, article2Id, article3Id })
                }, x => x.ExcludeDomainEventTechnicalFields());
        }
    }

    public class Keywords2Tests
    {
        [Fact]
        public void Two_same_intersections_are_equals()
        {
            var firstIntersection = new Keywords2(new[] { new Keyword2("test", 2) });
            var secondIntersection = new Keywords2(new[] { new Keyword2("test", 5) });

            firstIntersection.Should().Be(secondIntersection);

            new[] { firstIntersection, secondIntersection }.GroupBy(x => x).Should().HaveCount(1);
        }
    }
}