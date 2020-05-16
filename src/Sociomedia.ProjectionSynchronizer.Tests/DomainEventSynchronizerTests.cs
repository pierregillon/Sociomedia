using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using FluentAssertions;
using LinqToDB;
using LinqToDB.Data;
using Sociomedia.Articles.Domain;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Medias.Domain;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ReadModel.DataAccess;
using Sociomedia.ReadModel.DataAccess.Tables;
using StructureMap;
using Xunit;

namespace Sociomedia.ProjectionSynchronizer.Tests
{
    public class DomainEventSynchronizerTests : AcceptanceTests
    {
        private readonly InMemoryBus _inMemoryBus = new InMemoryBus();
        private readonly DomainEventSynchronizer _synchronizer;
        private readonly DbConnectionReadModel _dbConnection;

        public DomainEventSynchronizerTests()
        {
            Container.Inject<IEventBus>(_inMemoryBus);
            Container.Inject<ILogger>(new EmptyLogger());

            var configuration = Container.GetInstance<ProjectionSynchronizationConfiguration>();
            configuration.ReconnectionDelayMs = 1;

            _synchronizer = Container.GetInstance<DomainEventSynchronizer>();
            _dbConnection = Container.GetInstance<DbConnectionReadModel>();
        }

        [Fact]
        public async Task Synchronization_start_by_default_with_no_last_position()
        {
            // Act
            await _synchronizer.StartSynchronization();

            // Asserts
            _inMemoryBus.LastStreamPosition
                .Should()
                .Be(null);
        }

        [Fact]
        public async Task Create_article_when_receiving_article_imported_event()
        {
            await _synchronizer.StartSynchronization();

            // Acts

            var articleSynchronized = new ArticleImported(
                Guid.NewGuid(),
                "My title",
                "This is a simple summary",
                new DateTimeOffset(2020, 05, 06, 10, 0, 0, TimeSpan.FromHours(2)),
                "https://test.com",
                "https://test/image/jpg",
                "externalId",
                Array.Empty<string>(),
                Guid.NewGuid()
            );

            await _inMemoryBus.Push(1, articleSynchronized);

            // Asserts

            var articles = await _dbConnection.Articles.ToArrayAsync();

            articles
                .Should()
                .BeEquivalentTo(new[] {
                    new ArticleTable {
                        Id = articleSynchronized.Id,
                        Title = articleSynchronized.Title,
                        ImageUrl = articleSynchronized.ImageUrl,
                        Url = articleSynchronized.Url,
                        Summary = articleSynchronized.Summary,
                        PublishDate = articleSynchronized.PublishDate,
                        MediaId = articleSynchronized.MediaId
                    }
                });
        }

        [Fact]
        public async Task Update_article_when_receiving_article_updated_event()
        {
            await _synchronizer.StartSynchronization();

            // Acts

            var articleSynchronized = new ArticleImported(
                Guid.NewGuid(),
                "My title",
                "This is a simple summary",
                new DateTimeOffset(2020, 05, 06, 10, 0, 0, TimeSpan.FromHours(2)),
                "https://test.com",
                "https://test/image/jpg",
                "externalId",
                Array.Empty<string>(),
                Guid.NewGuid()
            );

            var articleUpdated = new ArticleUpdated(
                articleSynchronized.Id,
                "My title 2",
                "This is a simple summary 2",
                new DateTimeOffset(2020, 05, 07, 10, 0, 0, TimeSpan.FromHours(2)),
                "https://test2.com",
                "https://test/image2/jpg"
            );

            await _inMemoryBus.Push(1, articleSynchronized);
            await _inMemoryBus.Push(2, articleUpdated);

            // Asserts

            var articles = await _dbConnection.Articles.ToArrayAsync();

            articles
                .Should()
                .BeEquivalentTo(new[] {
                    new ArticleTable {
                        Id = articleSynchronized.Id,
                        Title = articleUpdated.Title,
                        ImageUrl = articleUpdated.ImageUrl,
                        Url = articleUpdated.Url,
                        Summary = articleUpdated.Summary,
                        PublishDate = articleUpdated.PublishDate,
                        MediaId = articleSynchronized.MediaId
                    }
                });
        }

        [Fact]
        public async Task Create_keywords_when_receiving_article_synchronized_event()
        {
            await _synchronizer.StartSynchronization();

            // Acts
            var articleSynchronized = SomeArticleSynchronized(new[] { "coronavirus", "france", "pandemic" });

            await _inMemoryBus.Push(1, articleSynchronized);

            // Asserts
            var keywords = await _dbConnection.Keywords.ToArrayAsync();

            keywords
                .Should()
                .BeEquivalentTo(new[] {
                    new { FK_Article = articleSynchronized.Id, Value = "coronavirus" },
                    new { FK_Article = articleSynchronized.Id, Value = "france" },
                    new { FK_Article = articleSynchronized.Id, Value = "pandemic" },
                });
        }

        [Fact]
        public async Task Create_media_when_receiving_media_events()
        {
            await _synchronizer.StartSynchronization();

            // Acts
            var mediaId = Guid.NewGuid();

            await _inMemoryBus.Push(1, new MediaAdded(mediaId, "Liberation", "test", PoliticalOrientation.Center));
            await _inMemoryBus.Push(2, new MediaEdited(mediaId, "Libération", "test", PoliticalOrientation.Left));

            // Asserts

            (await _dbConnection.Medias.ToArrayAsync())
                .Should()
                .BeEquivalentTo(new[] {
                    new MediaTable {
                        Id = mediaId,
                        Name = "Libération",
                        ImageUrl = "test",
                        PoliticalOrientation = (int) PoliticalOrientation.Left
                    },
                });
        }

        [Fact]
        public async Task Create_media_feed_when_receiving_media_feed_events()
        {
            await _synchronizer.StartSynchronization();

            // Acts
            var mediaId = Guid.NewGuid();

            await _inMemoryBus.Push(1, new MediaAdded(mediaId, "Liberation", "test", PoliticalOrientation.Center));
            await _inMemoryBus.Push(2, new MediaFeedAdded(mediaId, "https://test/myfeed.xml"));
            await _inMemoryBus.Push(3, new MediaFeedAdded(mediaId, "https://test/myfeed2.xml"));
            await _inMemoryBus.Push(4, new MediaFeedRemoved(mediaId, "https://test/myfeed2.xml"));
            await _inMemoryBus.Push(5, new MediaFeedSynchronized(mediaId, "https://test/myfeed.xml", DateTime.Today));

            // Asserts

            (await _dbConnection.MediaFeeds.ToArrayAsync())
                .Should()
                .BeEquivalentTo(new[] {
                    new MediaFeedTable {
                        MediaId = mediaId,
                        FeedUrl = "https://test/myfeed.xml",
                        SynchronizationDate = DateTime.Today
                    },
                });
        }

        [Fact]
        public async Task Delete_media_when_receiving_media_deleted()
        {
            await _synchronizer.StartSynchronization();

            // Acts
            var mediaId = Guid.NewGuid();

            await _inMemoryBus.Push(1, new MediaAdded(mediaId, "Liberation", "test", PoliticalOrientation.Center));
            await _inMemoryBus.Push(2, new MediaDeleted(mediaId));

            // Asserts

            (await _dbConnection.Medias.ToArrayAsync())
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Delete_media_feeds_when_receiving_media_deleted()
        {
            await _synchronizer.StartSynchronization();

            // Acts
            var mediaId = Guid.NewGuid();

            await _inMemoryBus.Push(1, new MediaAdded(mediaId, "Liberation", "test", PoliticalOrientation.Center));
            await _inMemoryBus.Push(2, new MediaFeedAdded(mediaId, "https://test/myfeed.xml"));
            await _inMemoryBus.Push(3, new MediaDeleted(mediaId));

            // Asserts

            (await _dbConnection.MediaFeeds.ToArrayAsync())
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Delete_articles_and_keywords_when_receiving_media_deleted()
        {
            await _synchronizer.StartSynchronization();

            // Acts
            var mediaId = Guid.NewGuid();

            await _inMemoryBus.Push(1, new MediaAdded(mediaId, "Liberation", "test", PoliticalOrientation.Center));
            await _inMemoryBus.Push(2, new MediaFeedAdded(mediaId, "https://test/myfeed.xml"));
            await _inMemoryBus.Push(3, new ArticleImported(
                Guid.NewGuid(),
                "My title",
                "This is a simple summary",
                new DateTimeOffset(2020, 05, 06, 10, 0, 0, TimeSpan.FromHours(2)),
                "https://test.com",
                "https://test/image/jpg",
                "externalId",
                Array.Empty<string>(),
                mediaId
            ));
            await _inMemoryBus.Push(3, new MediaDeleted(mediaId));

            // Asserts

            (await _dbConnection.Articles.ToArrayAsync())
                .Should()
                .BeEmpty();
            (await _dbConnection.Keywords.ToArrayAsync())
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Update_last_position_in_stream_for_each_events()
        {
            await _synchronizer.StartSynchronization();

            await _inMemoryBus.Push(1, SomeArticleSynchronized());
            await _inMemoryBus.Push(2, SomeArticleSynchronized());
            await _inMemoryBus.Push(3, SomeArticleSynchronized());

            _dbConnection.SynchronizationInformation
                .Single()
                .LastPosition
                .Should()
                .Be(3);

            _dbConnection.SynchronizationInformation
                .Single()
                .LastUpdateDate
                .GetValueOrDefault()
                .Date
                .Should()
                .Be(DateTime.Now.Date);
        }

        // ----- Internal logic

        private static ArticleImported SomeArticleSynchronized(string[] keywords = null)
        {
            return new ArticleImported(
                Guid.NewGuid(),
                "My title",
                "This is a simple summary",
                new DateTimeOffset(2020, 05, 06, 10, 0, 0, TimeSpan.FromHours(2)),
                "https://test.com",
                "https://test/image/jpg",
                "externalId",
                keywords ?? new string[0],
                Guid.NewGuid()
            );
        }
    }

    public class AcceptanceTests : IDisposable
    {
        protected readonly IContainer Container;
        private readonly string _databaseName = "database-" + Guid.NewGuid() + ".sqlite";
        private string FullPath => Path.Combine(Directory.GetCurrentDirectory(), _databaseName);

        public AcceptanceTests()
        {
            File.Copy("./database.db", FullPath);

            Container = ContainerBuilder.Build(new Configuration {
                SqlDatabase = new SqlDatabaseConfiguration {
                    ProviderName = "SQLite",
                    ConnectionString = "Data Source=" + FullPath
                }
            });

            DataConnection.DefaultSettings = Container.GetInstance<DbSettings>();

            InitDatabase();
        }

        public void Dispose()
        {
            Container?.Dispose();
            File.Delete(FullPath);
        }

        private void InitDatabase()
        {
            var db = Container.GetInstance<DbConnectionReadModel>();

            db.GenerateMissingTables();
            db.ClearDatabase();
        }
    }
}