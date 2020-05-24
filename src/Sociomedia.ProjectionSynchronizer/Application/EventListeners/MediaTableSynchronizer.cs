using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.Core.Application;
using Sociomedia.Medias.Domain;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.ProjectionSynchronizer.Application.EventListeners
{
    public class MediaTableSynchronizer :
        IEventListener<MediaAdded>,
        IEventListener<MediaEdited>,
        IEventListener<MediaFeedAdded>,
        IEventListener<MediaFeedRemoved>,
        IEventListener<MediaFeedSynchronized>,
        IEventListener<MediaDeleted>
    {
        private readonly DbConnectionReadModel _dbConnection;

        public MediaTableSynchronizer(DbConnectionReadModel dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task On(MediaAdded @event)
        {
            await _dbConnection.Medias
                .Value(x => x.Id, @event.Id)
                .Value(x => x.Name, @event.Name)
                .Value(x => x.ImageUrl, @event.ImageUrl)
                .Value(x => x.PoliticalOrientation, (int) @event.PoliticalOrientation)
                .InsertAsync();
        }

        public async Task On(MediaEdited @event)
        {
            await _dbConnection.Medias
                .Where(x => x.Id == @event.Id)
                .Set(x => x.Name, @event.Name)
                .Set(x => x.ImageUrl, @event.ImageUrl)
                .Set(x => x.PoliticalOrientation, (int) @event.PoliticalOrientation)
                .UpdateAsync();
        }

        public async Task On(MediaFeedAdded @event)
        {
            await _dbConnection.MediaFeeds
                .Value(x => x.MediaId, @event.Id)
                .Value(x => x.FeedUrl, @event.FeedUrl ?? "")
                .InsertAsync();
        }

        public async Task On(MediaFeedRemoved @event)
        {
            await _dbConnection.MediaFeeds
                .Where(x => x.MediaId == @event.Id && x.FeedUrl == @event.FeedUrl)
                .DeleteAsync();
        }

        public async Task On(MediaFeedSynchronized @event)
        {
            await _dbConnection.MediaFeeds
                .Where(x => x.MediaId == @event.Id && x.FeedUrl == @event.FeedUrl)
                .Set(x => x.SynchronizationDate, @event.SynchronizationDate)
                .UpdateAsync();
        }

        public async Task On(MediaDeleted @event)
        {
            await _dbConnection.Medias
                .Where(x => x.Id == @event.Id)
                .DeleteAsync();

            await _dbConnection.MediaFeeds
                .Where(x => x.MediaId == @event.Id)
                .DeleteAsync();
        }
    }
}