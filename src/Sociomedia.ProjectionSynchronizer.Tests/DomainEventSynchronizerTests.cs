using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using FluentAssertions;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Data.Sqlite;
using Sociomedia.Domain.Articles;
using Sociomedia.Domain.Medias;
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
        public async Task Update_last_position_in_stream_for_each_events()
        {
            await _synchronizer.StartSynchronization();

            await _inMemoryBus.Push(1, SomeArticleSynchronized());
            await _inMemoryBus.Push(2, SomeArticleSynchronized());
            await _inMemoryBus.Push(3, SomeArticleSynchronized());

            this._dbConnection.SynchronizationInformation
                .Single()
                .LastPosition
                .Should()
                .Be(3);

            this._dbConnection.SynchronizationInformation
                .Single()
                .LastUpdateDate
                .GetValueOrDefault()
                .Date
                .Should()
                .Be(DateTime.UtcNow.Date);
        }

        [Fact]
        public async Task Restart_connection_on_connection_lost()
        {
            await _synchronizer.StartSynchronization();

            await _inMemoryBus.SimulateConnectionLost();

            _inMemoryBus.IsListening
                .Should()
                .Be(true);
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

            var tableTypes = db.GetType()
                .Properties()
                .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(ITable<>))
                .Select(x => x.PropertyType.GenericTypeArguments[0])
                .ToArray();

            foreach (var tableType in tableTypes) {
                db.CreateTableIfDoesNotExists(tableType);
                db.DeleteAllFromTable(tableType);
            }
        }
    }

    public static class DataConnectionExtensions
    {
        public static void CreateTableIfDoesNotExists(this DataConnection dbConnection, Type tableType)
        {
            typeof(DataConnectionExtensions)
                .GetMethod(nameof(CreateTableIfDoesNotExists), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(tableType)
                .Invoke(null, new[] { dbConnection });
        }

        public static void DeleteAllFromTable(this DataConnection dbConnection, Type tableType)
        {
            typeof(DataConnectionExtensions)
                .GetMethod(nameof(DeleteAllFromTable), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(tableType)
                .Invoke(null, new[] { dbConnection });
        }

        private static void CreateTableIfDoesNotExists<T>(DataConnection dbConnection) where T : class
        {
            try {
                dbConnection.GetTable<T>().Count();
            }
            catch (SqliteException e) {
                if (e.SqliteErrorCode == 1) {
                    dbConnection.CreateTable<T>();
                }
            }
        }

        private static void DeleteAllFromTable<T>(DataConnection dbConnection) where T : class
        {
            dbConnection
                .GetTable<T>()
                .Where(x => true)
                .Delete();
        }
    }
}