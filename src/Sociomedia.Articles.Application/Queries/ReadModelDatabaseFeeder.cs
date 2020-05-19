using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Sociomedia.Articles.Domain;
using Sociomedia.Core.Application;
using Sociomedia.Medias.Domain;

namespace Sociomedia.Articles.Application.Queries
{
    public class ReadModelDatabaseFeeder :
        IEventListener<MediaFeedAdded>,
        IEventListener<MediaFeedSynchronized>,
        IEventListener<MediaFeedRemoved>,
        IEventListener<ArticleImported>,
        IEventListener<ArticleUpdated>,
        IEventListener<MediaDeleted>,
        IEventListener<ArticleDeleted>
    {
        private readonly InMemoryDatabase _database;
        private readonly ILogger _logger;

        public ReadModelDatabaseFeeder(InMemoryDatabase database, ILogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public Task On(MediaFeedAdded @event)
        {
            _database.Add(new MediaFeedReadModel {
                MediaId = @event.Id,
                FeedUrl = @event.FeedUrl,
                LastSynchronizationDate = null
            });

            return Task.CompletedTask;
        }

        public Task On(MediaFeedSynchronized @event)
        {
            var source = _database
                .List<MediaFeedReadModel>()
                .SingleOrDefault(x => x.MediaId == @event.Id && x.FeedUrl == @event.FeedUrl);

            if (source != null) {
                source.LastSynchronizationDate = @event.SynchronizationDate;
            }

            return Task.CompletedTask;
        }

        public Task On(MediaFeedRemoved @event)
        {
            var source = _database
                .List<MediaFeedReadModel>()
                .SingleOrDefault(x => x.MediaId == @event.Id && x.FeedUrl == @event.FeedUrl);

            _database.Remove(source);

            return Task.CompletedTask;
        }

        public Task On(ArticleImported @event)
        {
            _database.Add(new ArticleReadModel {
                MediaId = @event.MediaId,
                ExternalArticleId = @event.ExternalArticleId,
                ArticleId = @event.Id,
                PublishDate = @event.PublishDate
            });

            return Task.CompletedTask;
        }

        public Task On(ArticleUpdated @event)
        {
            var source = _database
                .List<ArticleReadModel>()
                .SingleOrDefault(x => x.ArticleId == @event.Id);

            if (source != null) {
                source.PublishDate = @event.PublishDate;
            }
            else {
                _logger.Error($"[READMODEL_DATABASE_FEEDER] Unable to finder article '{@event.Id}' to update.");
            }

            return Task.CompletedTask;
        }

        public Task On(MediaDeleted @event)
        {
            var feeds = _database
                .List<MediaFeedReadModel>()
                .Where(x => x.MediaId == @event.Id);

            foreach (var feed in feeds) {
                _database.Remove(feed);
            }

            return Task.CompletedTask;
        }

        public Task On(ArticleDeleted @event)
        {
            var articles = _database
                .List<ArticleReadModel>()
                .Where(x => x.ArticleId == @event.Id);

            foreach (var article in articles) {
                _database.Remove(article);
            }

            return Task.CompletedTask;
        }
    }
}