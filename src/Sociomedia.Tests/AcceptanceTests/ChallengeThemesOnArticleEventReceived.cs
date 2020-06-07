using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.CQRS;
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

            await EventPublisher.Publish(new[] {
                new ArticleKeywordsDefined(article1Id, new[] { AKeyword("coronavirus", 2), AKeyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { AKeyword("coronavirus", 3), AKeyword("chine", 2) }),
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 5) }, new[] { article1Id, article2Id })
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Add_article_to_existing_theme()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();
            var article3Id = Guid.NewGuid();

            await EventPublisher.Publish(new[] {
                new ArticleKeywordsDefined(article1Id, new[] { AKeyword("coronavirus", 2), AKeyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { AKeyword("coronavirus", 3), AKeyword("chine", 2) }),
                new ArticleKeywordsDefined(article3Id, new[] { AKeyword("coronavirus", 3), AKeyword("italie", 3) })
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 5) }, new[] { article1Id, article2Id }),
                    new ArticleAddedToTheme(default, article3Id),
                    new ThemeKeywordsUpdated(default, new[] { new Keyword("coronavirus", 8) })
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Create_2_themes_on_article_keywords_defined()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();
            var article3Id = Guid.NewGuid();

            await EventPublisher.Publish(new[] {
                new ArticleKeywordsDefined(article1Id, new[] { AKeyword("coronavirus", 2), AKeyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { AKeyword("opera", 3), AKeyword("chine", 2) }),
                new ArticleKeywordsDefined(article3Id, new[] { AKeyword("coronavirus", 5), AKeyword("chine", 3) }),
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 7) }, new[] { article1Id, article3Id }),
                    new ThemeAdded(default, new[] { new Keyword("chine", 5) }, new[] { article2Id, article3Id }),
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Create_a_new_theme_on_keyword_theme_intersection()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();
            var article3Id = Guid.NewGuid();

            await EventPublisher.Publish(new[] {
                new ArticleKeywordsDefined(article1Id, new[] { AKeyword("coronavirus", 2), AKeyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { AKeyword("coronavirus", 3), AKeyword("france", 3) }),
                new ArticleKeywordsDefined(article3Id, new[] { AKeyword("coronavirus", 5), AKeyword("chine", 3) }),
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 5), new Keyword("france", 5) }, new[] { article1Id, article2Id }),
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 10) }, new[] { article1Id, article2Id, article3Id })
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Create_multiple_themes()
        {
            var events = new[] {
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { AKeyword("coronavirus", 2), AKeyword("france", 2) }),
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { AKeyword("coronavirus", 8), AKeyword("chine", 3) }),
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { AKeyword("italie", 5), AKeyword("france", 5) }),
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { AKeyword("chine", 5), AKeyword("opera", 3) }),
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { AKeyword("coronavirus", 2), AKeyword("italie", 6) }),
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { AKeyword("coronavirus", 5) }),
            };

            await EventPublisher.Publish(events);

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 10) }, new[] { events.ElementAt(0).Id, events.ElementAt(1).Id }),
                    new ThemeAdded(default, new[] { new Keyword("france", 7) }, new[] { events.ElementAt(0).Id, events.ElementAt(2).Id }),
                    new ThemeAdded(default, new[] { new Keyword("chine", 8) }, new[] { events.ElementAt(1).Id, events.ElementAt(3).Id }),
                    new ArticleAddedToTheme(default, events.ElementAt(4).Id),
                    new ThemeKeywordsUpdated(default, new[] { new Keyword("coronavirus", 12) }),
                    new ThemeAdded(default, new[] { new Keyword("italie", 8) }, new[] { events.ElementAt(2).Id, events.ElementAt(4).Id }),
                    new ArticleAddedToTheme(default, events.ElementAt(5).Id),
                    new ThemeKeywordsUpdated(default, new[] { new Keyword("coronavirus", 17) }),
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Create_multiple_themes_with_same_theme_creation_in_article_intersection_and_theme_intersection()
        {
            var events = new[] {
                KeywordDefined("a", "b", "c", "1"),
                KeywordDefined("a", "b", "c", "2"),
                KeywordDefined("a", "c", "3"),
                KeywordDefined("b", "c", "4"),
                KeywordDefined("a", "b", "c"),
            };

            await EventPublisher.Publish(events);

            var newEvents = await EventStore.GetNewEvents().Pipe(x => x.ToArray());

            newEvents
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(newEvents.ElementAt(0).Id, new[] { new Keyword("a", 4), new Keyword("b", 4), new Keyword("c", 4) }, new[] { events.ElementAt(0).Id, events.ElementAt(1).Id }),
                    new ThemeAdded(newEvents.ElementAt(1).Id, new[] { new Keyword("a", 6), new Keyword("c", 6) }, new[] { events.ElementAt(0).Id, events.ElementAt(1).Id, events.ElementAt(2).Id }),
                    new ThemeAdded(newEvents.ElementAt(2).Id, new[] { new Keyword("b", 6), new Keyword("c", 6) }, new[] { events.ElementAt(0).Id, events.ElementAt(1).Id, events.ElementAt(3).Id }),
                    new ThemeAdded(newEvents.ElementAt(3).Id, new[] { new Keyword("c", 8) }, new[] { events.ElementAt(0).Id, events.ElementAt(1).Id, events.ElementAt(2).Id, events.ElementAt(3).Id }),
                    new ArticleAddedToTheme(newEvents.ElementAt(0).Id, events.ElementAt(4).Id ), 
                    new ThemeKeywordsUpdated(newEvents.ElementAt(0).Id, new []{ new Keyword("a", 6), new Keyword("b", 6), new Keyword("c", 6)}),
                    new ArticleAddedToTheme(newEvents.ElementAt(1).Id, events.ElementAt(4).Id ),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(1).Id, new []{ new Keyword("a", 8), new Keyword("c", 8)}),
                    new ArticleAddedToTheme(newEvents.ElementAt(2).Id, events.ElementAt(4).Id ),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(2).Id, new []{ new Keyword("b", 8), new Keyword("c", 8)}),
                    new ArticleAddedToTheme(newEvents.ElementAt(3).Id, events.ElementAt(4).Id ), 
                    new ThemeKeywordsUpdated(newEvents.ElementAt(3).Id, new []{ new Keyword("c", 10)}),
                }, x => x.ExcludeDomainEventTechnicalFields2());
        }

        [Fact]
        public async Task Several_themes_have_the_same_intersection_with_an_articles()
        {
            var articleEvents = new[] {
                KeywordDefined("a", "b", "1"),
                KeywordDefined("a", "b", "1"),
                KeywordDefined("a", "b", "2"),
                KeywordDefined("a", "b"),
            };

            await EventPublisher.Publish(articleEvents);

            var newEvents = await EventStore.GetNewEvents().Pipe(x => x.ToArray());

            newEvents
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(newEvents.ElementAt(0).Id, new[] { new Keyword("a", 4), new Keyword("b", 4), new Keyword("1", 2), }, new[] { articleEvents.ElementAt(0).Id, articleEvents.ElementAt(1).Id }),
                    new ThemeAdded(newEvents.ElementAt(1).Id, new[] { new Keyword("a", 6), new Keyword("b", 6)}, new[] { articleEvents.ElementAt(0).Id, articleEvents.ElementAt(1).Id, articleEvents.ElementAt(2).Id }),
                    new ArticleAddedToTheme(newEvents.ElementAt(1).Id, articleEvents.ElementAt(3).Id),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(1).Id, new[] { new Keyword("a", 8), new Keyword("b", 8)}),
                }, x => x.ExcludeDomainEventTechnicalFields2());
        }

        private static ArticleKeywordsDefined KeywordDefined(params string[] keywords)
        {
            return new ArticleKeywordsDefined(Guid.NewGuid(), keywords.Select(x => AKeyword(x, 2)).ToArray());
        }

        private static Articles.Domain.Keywords.Keyword AKeyword(string value, int occurence)
        {
            return new Articles.Domain.Keywords.Keyword(value, occurence);
        }
    }
}