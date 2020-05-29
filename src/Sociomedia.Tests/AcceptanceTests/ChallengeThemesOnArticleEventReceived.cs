using System;
using System.Threading.Tasks;
using FluentAssertions;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Domain.Keywords;
using Sociomedia.Core.Domain;
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

            EventStore.EnableRepublish();

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
    }
}