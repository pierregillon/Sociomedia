using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Themes.Domain;
using Xunit;

namespace Sociomedia.Tests.AcceptanceTests
{
    public class ChallengeThemesOnArticleEventReceived : AcceptanceTests
    {
        private readonly IClock _clock = Substitute.For<IClock>();
        
        public ChallengeThemesOnArticleEventReceived()
        {
            _clock.Now().Returns(DateTimeOffset.Now);

            Container.Inject(_clock);
        }

        [Fact]
        public async Task Article_without_keywords_are_ignored()
        {
            var articleId = Guid.NewGuid();

            await EventPublisher.Publish(new DomainEvent[] {
                AnArticleImported(articleId),
                AnArticleImported(Guid.NewGuid()),
                AnArticleImported(Guid.NewGuid()),
                new ArticleKeywordsDefined(articleId, new []{AKeyword("coronavirus", 2)})
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Create_new_theme_on_article_keywords_defined_with_common_keywords()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();

            await EventPublisher.Publish(new[] {
                AnArticleWithKeywords(article1Id, AKeyword("coronavirus", 2), AKeyword("france", 2)),
                AnArticleWithKeywords(article2Id, AKeyword("coronavirus", 3), AKeyword("chine", 2))
            }.SelectMany(x => x));

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
                AnArticleWithKeywords(article1Id, AKeyword("coronavirus", 2), AKeyword("france", 2)),
                AnArticleWithKeywords(article2Id, AKeyword("coronavirus", 3), AKeyword("chine", 2)),
                AnArticleWithKeywords(article3Id, AKeyword("coronavirus", 3), AKeyword("italie", 3))
            }.SelectMany(x => x));

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 5) }, new[] { article1Id, article2Id }),
                    new ArticleAddedToTheme(default, article3Id),
                    new ThemeKeywordsUpdated(default, new[] { new Keyword("coronavirus", 8) })
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Create_new_theme_ordering_keywords_by_occurence_then_value()
        {
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();
            var article3 = Guid.NewGuid();

            var events = new[] {
                AnArticleWithKeywords(article1, KeywordDefined("b", "a", "1")),
                AnArticleWithKeywords(article2, KeywordDefined("a", "b", "2")),
                AnArticleWithKeywords(article3, KeywordDefined("b", "a")),
            }.SelectMany(x => x);

            await EventPublisher.Publish(events);

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("a", 4), new Keyword("b", 4) }, new[] { article1, article2 }),
                    new ArticleAddedToTheme(default, article3),
                    new ThemeKeywordsUpdated(default, new[] { new Keyword("a", 6), new Keyword("b", 6) })
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Create_2_themes_on_article_keywords_defined()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();
            var article3Id = Guid.NewGuid();

            await EventPublisher.Publish(new[] {
                AnArticleWithKeywords(article1Id, AKeyword("coronavirus", 2), AKeyword("france", 2)),
                AnArticleWithKeywords(article2Id, AKeyword("opera", 3), AKeyword("chine", 2)),
                AnArticleWithKeywords(article3Id, AKeyword("coronavirus", 5), AKeyword("chine", 3)),
            }.SelectMany(x => x));

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
                AnArticleWithKeywords(article1Id, AKeyword("coronavirus", 2), AKeyword("france", 2)),
                AnArticleWithKeywords(article2Id, AKeyword("coronavirus", 3), AKeyword("france", 3)),
                AnArticleWithKeywords(article3Id, AKeyword("coronavirus", 5), AKeyword("chine", 3)),
            }.SelectMany(x => x));

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
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();
            var article3 = Guid.NewGuid();
            var article4 = Guid.NewGuid();
            var article5 = Guid.NewGuid();
            var article6 = Guid.NewGuid();

            var events = new[] {
                AnArticleWithKeywords(article1, AKeyword("coronavirus", 2), AKeyword("france", 2)),
                AnArticleWithKeywords(article2, AKeyword("coronavirus", 8), AKeyword("chine", 3)),
                AnArticleWithKeywords(article3, AKeyword("italie", 5), AKeyword("france", 5)),
                AnArticleWithKeywords(article4, AKeyword("chine", 5), AKeyword("opera", 3)),
                AnArticleWithKeywords(article5, AKeyword("coronavirus", 2), AKeyword("italie", 6)),
                AnArticleWithKeywords(article6, AKeyword("coronavirus", 5)),
            }.SelectMany(x => x);

            await EventPublisher.Publish(events);

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 10) }, new[] { article1, article2 }),
                    new ThemeAdded(default, new[] { new Keyword("france", 7) }, new[] { article1, article3 }),
                    new ThemeAdded(default, new[] { new Keyword("chine", 8) }, new[] { article2, article4 }),
                    new ArticleAddedToTheme(default, article5),
                    new ThemeKeywordsUpdated(default, new[] { new Keyword("coronavirus", 12) }),
                    new ThemeAdded(default, new[] { new Keyword("italie", 8) }, new[] { article3, article5 }),
                    new ArticleAddedToTheme(default, article6),
                    new ThemeKeywordsUpdated(default, new[] { new Keyword("coronavirus", 17) }),
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Create_multiple_themes_with_same_theme_creation_in_article_intersection_and_theme_intersection()
        {
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();
            var article3 = Guid.NewGuid();
            var article4 = Guid.NewGuid();
            var article5 = Guid.NewGuid();

            var events = new[] {
                AnArticleWithKeywords(article1, KeywordDefined("a", "b", "c", "1")),
                AnArticleWithKeywords(article2, KeywordDefined("a", "b", "c", "2")),
                AnArticleWithKeywords(article3, KeywordDefined("a", "c", "3")),
                AnArticleWithKeywords(article4, KeywordDefined("b", "c", "4")),
                AnArticleWithKeywords(article5, KeywordDefined("a", "b", "c")),
            }.SelectMany(x => x);

            await EventPublisher.Publish(events);

            var newEvents = await EventStore.GetNewEvents().Pipe(x => x.ToArray());

            newEvents
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(newEvents.ElementAt(0).Id, new[] { new Keyword("a", 4), new Keyword("b", 4), new Keyword("c", 4) }, new[] { article1, article2 }),
                    new ThemeAdded(newEvents.ElementAt(1).Id, new[] { new Keyword("a", 6), new Keyword("c", 6) }, new[] { article1, article2, article3 }),
                    new ThemeAdded(newEvents.ElementAt(2).Id, new[] { new Keyword("b", 6), new Keyword("c", 6) }, new[] { article1, article2, article4 }),
                    new ThemeAdded(newEvents.ElementAt(3).Id, new[] { new Keyword("c", 8) }, new[] { article1, article2, article3, article4 }),
                    new ArticleAddedToTheme(newEvents.ElementAt(0).Id, article5),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(0).Id, new[] { new Keyword("a", 6), new Keyword("b", 6), new Keyword("c", 6) }),
                    new ArticleAddedToTheme(newEvents.ElementAt(1).Id, article5),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(1).Id, new[] { new Keyword("a", 8), new Keyword("c", 8) }),
                    new ArticleAddedToTheme(newEvents.ElementAt(2).Id, article5),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(2).Id, new[] { new Keyword("b", 8), new Keyword("c", 8) }),
                    new ArticleAddedToTheme(newEvents.ElementAt(3).Id, article5),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(3).Id, new[] { new Keyword("c", 10) }),
                }, x => x.ExcludeDomainEventTechnicalFields2());
        }

        [Fact]
        public async Task Keywords_intersection_with_themes_ignores_keyword_intersection_order()
        {
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();
            var article3 = Guid.NewGuid();
            var article4 = Guid.NewGuid();
            var article5 = Guid.NewGuid();

            // Arrange

            await EventPublisher.Publish(new[] {
                AnArticleWithKeywords(article1, KeywordDefined("b", "c")),
                AnArticleWithKeywords(article2, KeywordDefined("b", "c")),
                new DomainEvent[] {
                    AnArticleImported(article3),
                    new ArticleKeywordsDefined(article3, new[] {
                        new Articles.Domain.Keywords.Keyword("c", 6),
                        new Articles.Domain.Keywords.Keyword("a", 5),
                        new Articles.Domain.Keywords.Keyword("b", 4),
                    })
                },
                new DomainEvent[] {
                    AnArticleImported(article4),
                    new ArticleKeywordsDefined(article4, new[] {
                        new Articles.Domain.Keywords.Keyword("c", 7),
                        new Articles.Domain.Keywords.Keyword("a", 6),
                        new Articles.Domain.Keywords.Keyword("b", 5),
                    })
                }
            }.SelectMany(x => x));

            // Act

            await EventPublisher.Publish(AnArticleWithKeywords(article5, KeywordDefined("c", "b")));

            // Assert

            var newEvents = await EventStore.GetNewEvents().Pipe(x => x.ToArray());

            newEvents
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(newEvents.ElementAt(0).Id, new[] { new Keyword("b", 4), new Keyword("c", 4)}, new[] { article1, article2 }),
                    new ArticleAddedToTheme(newEvents.ElementAt(0).Id, article3),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(0).Id, new[] { new Keyword("b", 8), new Keyword("c", 10) }),
                    new ArticleAddedToTheme(newEvents.ElementAt(0).Id, article4),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(0).Id, new[] { new Keyword("b", 13), new Keyword("c", 17) }),
                    new ThemeAdded(newEvents.ElementAt(5).Id, new[] { new Keyword("c", 13), new Keyword("a", 11), new Keyword("b", 9)}, new[] { article3, article4}),
                    new ArticleAddedToTheme(newEvents.ElementAt(0).Id, article5),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(0).Id, new[] { new Keyword("b", 15), new Keyword("c", 19) }),
                }, x => x.ExcludeDomainEventTechnicalFields2());
        }

        [Fact]
        public async Task Create_theme_when_keyword_intersection_not_all_contained_in_existing_theme()
        {
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();
            var article3 = Guid.NewGuid();
            var article4 = Guid.NewGuid();

            var events = new[] {
                AnArticleWithKeywords(article1, KeywordDefined("b", "c")),
                AnArticleWithKeywords(article2, KeywordDefined("b", "c")),
                AnArticleWithKeywords(article3, KeywordDefined("a", "b", "c")),
                AnArticleWithKeywords(article4, KeywordDefined("a", "b", "c")),
            }.SelectMany(x => x);

            await EventPublisher.Publish(events);

            var newEvents = await EventStore.GetNewEvents().Pipe(x => x.ToArray());

            newEvents
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(newEvents.ElementAt(0).Id, new[] { new Keyword("b", 4), new Keyword("c", 4)}, new[] { article1, article2 }),
                    new ArticleAddedToTheme(newEvents.ElementAt(0).Id, article3),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(0).Id, new[] { new Keyword("b", 6), new Keyword("c", 6) }),
                    new ArticleAddedToTheme(newEvents.ElementAt(0).Id, article4),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(0).Id, new[] { new Keyword("b", 8), new Keyword("c", 8) }),
                    new ThemeAdded(newEvents.ElementAt(5).Id, new[] { new Keyword("a", 4), new Keyword("b", 4), new Keyword("c", 4) }, new[] { article3, article4 }),

                }, x => x.ExcludeDomainEventTechnicalFields2());
        }

        [Fact]
        public async Task Several_themes_have_the_same_intersection_with_an_articles()
        {
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();
            var article3 = Guid.NewGuid();
            var article4 = Guid.NewGuid();
            var article5 = Guid.NewGuid();

            var articleEvents = new[] {
                AnArticleWithKeywords(article1, KeywordDefined("a", "b", "c")),
                AnArticleWithKeywords(article2, KeywordDefined("a", "b", "c")),
                AnArticleWithKeywords(article3, KeywordDefined("a", "b")),
                AnArticleWithKeywords(article4, KeywordDefined("a", "b")),
                AnArticleWithKeywords(article5, KeywordDefined("a")),
            }.SelectMany(x => x);

            await EventPublisher.Publish(articleEvents);

            var newEvents = await EventStore.GetNewEvents().Pipe(x => x.ToArray());

            newEvents
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(newEvents.ElementAt(0).Id, new[] { new Keyword("a", 4), new Keyword("b", 4), new Keyword("c", 2), }, new[] { article1, article2 }),
                    new ThemeAdded(newEvents.ElementAt(1).Id, new[] { new Keyword("a", 6), new Keyword("b", 6) }, new[] { article1, article2, article3 }),
                    new ArticleAddedToTheme(newEvents.ElementAt(1).Id, article4),
                    new ThemeKeywordsUpdated(newEvents.ElementAt(1).Id, new[] { new Keyword("a", 8), new Keyword("b", 8) }),
                    new ThemeAdded(newEvents.ElementAt(2).Id, new[] { new Keyword("a", 10) }, new[] { article1, article2, article3, article4, article5 }),
                }, x => x.ExcludeDomainEventTechnicalFields2());
        }

        [Theory]
        [InlineData(40)]
        [InlineData(33)]
        public async Task Articles_imported_with_a_publish_date_older_than_1_month_ago_are_ignored(int daysFromToday)
        {
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();

            await EventPublisher.Publish(new[] {
                AnArticleWithKeywordsAndDate(article1, _clock.Now().Subtract(TimeSpan.FromDays(daysFromToday)), AKeyword("coronavirus", 2), AKeyword("france", 2)),
                AnArticleWithKeywordsAndDate(article2, _clock.Now(), AKeyword("coronavirus", 3), AKeyword("chine", 2)),
            }.SelectMany(x => x));

            (await EventStore.GetNewEvents())
                .Should()
                .BeEmpty();
        }

        // ----- Privates

        private static Articles.Domain.Keywords.Keyword[] KeywordDefined(params string[] keywords)
        {
            return keywords.Select(x => AKeyword(x, 2)).ToArray();
        }

        private static Articles.Domain.Keywords.Keyword AKeyword(string value, int occurence)
        {
            return new Articles.Domain.Keywords.Keyword(value, occurence);
        }

        private ArticleImported AnArticleImported(Guid articleId, DateTimeOffset publishDate = default)
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

        public IEnumerable<DomainEvent> AnArticleWithKeywords(Guid id, params Articles.Domain.Keywords.Keyword[] keywords)
        {
            yield return AnArticleImported(id);
            yield return new ArticleKeywordsDefined(id, keywords);
        }

        public IEnumerable<DomainEvent> AnArticleWithKeywordsAndDate(Guid id, DateTimeOffset publishDate, params Articles.Domain.Keywords.Keyword[] keywords)
        {
            yield return AnArticleImported(id, publishDate);
            yield return new ArticleKeywordsDefined(id, keywords);
        }
    }
}