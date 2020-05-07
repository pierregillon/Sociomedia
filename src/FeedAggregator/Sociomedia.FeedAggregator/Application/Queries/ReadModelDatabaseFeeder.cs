using System.Linq;
using System.Threading.Tasks;
using Sociomedia.Application;
using Sociomedia.Domain.Medias;

namespace Sociomedia.FeedAggregator.Application.Queries
{
    public class ReadModelDatabaseFeeder :
        IEventListener<MediaFeedAdded>,
        IEventListener<MediaFeedSynchronized>,
        IEventListener<MediaFeedRemoved>
    {
        private readonly InMemoryDatabase _database;

        public ReadModelDatabaseFeeder(InMemoryDatabase database)
        {
            _database = database;
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
    }
}