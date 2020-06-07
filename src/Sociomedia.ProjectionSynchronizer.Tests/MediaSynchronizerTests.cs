using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using FluentAssertions;
using LinqToDB;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Medias.Domain;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ReadModel.DataAccess;
using Sociomedia.ReadModel.DataAccess.Tables;
using Xunit;

namespace Sociomedia.ProjectionSynchronizer.Tests
{
    public class MediaSynchronizerTests : AcceptanceTests
    {
        private readonly InMemoryBus _inMemoryBus = new InMemoryBus();
        private readonly DomainEventSynchronizer _synchronizer;
        private readonly DbConnectionReadModel _dbConnection;

        public MediaSynchronizerTests()
        {
            Container.Inject<IEventBus>(_inMemoryBus);
            Container.Inject<ILogger>(new EmptyLogger());

            var configuration = Container.GetInstance<ProjectionSynchronizationConfiguration>();
            configuration.ReconnectionDelayMs = 1;

            _synchronizer = Container.GetInstance<DomainEventSynchronizer>();
            _dbConnection = Container.GetInstance<DbConnectionReadModel>();
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
                        FeedUrl = "https://test/myfeed.xml"
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
    }
}