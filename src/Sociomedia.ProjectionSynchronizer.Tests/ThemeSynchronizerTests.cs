using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using FluentAssertions;
using LinqToDB;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ReadModel.DataAccess;
using Sociomedia.ReadModel.DataAccess.Tables;
using Sociomedia.Themes.Domain;
using Xunit;

namespace Sociomedia.ProjectionSynchronizer.Tests
{
    public class ThemeSynchronizerTests : AcceptanceTests
    {
        private readonly InMemoryBus _inMemoryBus = new InMemoryBus();
        private readonly DomainEventSynchronizer _synchronizer;
        private readonly DbConnectionReadModel _dbConnection;

        public ThemeSynchronizerTests()
        {
            Container.Inject<IEventBus>(_inMemoryBus);
            Container.Inject<ILogger>(new EmptyLogger());

            var configuration = Container.GetInstance<ProjectionSynchronizationConfiguration>();
            configuration.ReconnectionDelayMs = 1;

            _synchronizer = Container.GetInstance<DomainEventSynchronizer>();
            _dbConnection = Container.GetInstance<DbConnectionReadModel>();
        }

        [Fact]
        public async Task Create_theme()
        {
            await _synchronizer.StartSynchronization();

            // Acts
            var theme = Guid.NewGuid();
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();

            await _inMemoryBus.Push(1, new ThemeAdded(theme, new[] { new Keyword("coronavirus", 10), new Keyword("france", 12) }, new[] { article1, article2 }));

            // Asserts

            (await _dbConnection.Themes.ToArrayAsync())
                .Should()
                .BeEquivalentTo(new[] {
                    new ThemeTable {
                        Id = theme,
                        Name = "France, Coronavirus",
                        FullKeywords = "coronavirus (10) ; france (12)",
                        KeywordCount = 2
                    },
                });

            (await _dbConnection.ThemedArticles.ToArrayAsync())
                .Should()
                .BeEquivalentTo(new[] {
                    new ThemedArticleTable {
                        ThemeId = theme,
                        ArticleId = article1
                    },
                    new ThemedArticleTable {
                        ThemeId = theme,
                        ArticleId = article2
                    },
                });
        }

        [Fact]
        public async Task Add_article_to_theme()
        {
            await _synchronizer.StartSynchronization();

            // Acts
            var theme = Guid.NewGuid();
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();
            var article3 = Guid.NewGuid();

            await _inMemoryBus.Push(1, new ThemeAdded(theme, new[] { new Keyword("coronavirus", 10), new Keyword("france", 12) }, new[] { article1, article2 }));
            await _inMemoryBus.Push(1, new ArticleAddedToTheme(theme, article3));

            // Asserts

            (await _dbConnection.Themes.ToArrayAsync())
                .Should()
                .BeEquivalentTo(new[] {
                    new {
                        Id = theme,
                        Name = "France, Coronavirus",
                        FullKeywords = "coronavirus (10) ; france (12)"
                    },
                });

            (await _dbConnection.ThemedArticles.ToArrayAsync())
                .Should()
                .BeEquivalentTo(new[] {
                    new ThemedArticleTable {
                        ThemeId = theme,
                        ArticleId = article1
                    },
                    new ThemedArticleTable {
                        ThemeId = theme,
                        ArticleId = article2
                    },
                    new ThemedArticleTable {
                        ThemeId = theme,
                        ArticleId = article3
                    },
                });
        }

        [Fact]
        public async Task Name_keeps_only_four_first_keywords()
        {
            await _synchronizer.StartSynchronization();

            // Acts
            var theme = Guid.NewGuid();
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();

            await _inMemoryBus.Push(1, new ThemeAdded(theme, new[] {
                new Keyword("coronavirus", 3),
                new Keyword("france", 4),
                new Keyword("italie", 8),
                new Keyword("Pologne", 2),
                new Keyword("Issue", 20),
            }, new[] { article1, article2 }));

            // Asserts

            (await _dbConnection.Themes.ToArrayAsync())
                .Should()
                .BeEquivalentTo(new[] {
                    new {
                        Id = theme,
                        Name = "Issue, Italie, France, Coronavirus",
                    },
                });
        }

        [Fact]
        public async Task Update_theme_keywords()
        {
            await _synchronizer.StartSynchronization();

            // Acts
            var theme = Guid.NewGuid();
            var article1 = Guid.NewGuid();
            var article2 = Guid.NewGuid();

            await _inMemoryBus.Push(1, new ThemeAdded(theme, new[] { new Keyword("coronavirus", 10), new Keyword("france", 12) }, new[] { article1, article2 }));
            await _inMemoryBus.Push(1, new ThemeKeywordsUpdated(theme, new[] { new Keyword("coronavirus", 15), new Keyword("france", 14), new Keyword("bob", 2) }));

            // Asserts

            (await _dbConnection.Themes.ToArrayAsync())
                .Should()
                .BeEquivalentTo(new[] {
                    new ThemeTable {
                        Id = theme,
                        Name = "Coronavirus, France, Bob",
                        FullKeywords = "coronavirus (15) ; france (14) ; bob (2)",
                        KeywordCount = 3
                    },
                });
        }
    }
}