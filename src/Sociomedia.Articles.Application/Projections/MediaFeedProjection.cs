using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Sociomedia.Articles.Application.Queries;
using Sociomedia.Core.Application;
using Sociomedia.Medias.Domain;

namespace Sociomedia.Articles.Application.Projections
{
    public class MediaFeedProjection :
        Projection<MediaFeedReadModel>,
        IEventListener<MediaFeedAdded>,
        IEventListener<MediaFeedRemoved>,
        IEventListener<MediaDeleted>
    {
        public MediaFeedProjection(InMemoryDatabase database, ILogger logger) : base(database, logger) { }

        public Task On(MediaFeedAdded @event)
        {
            return Add(new MediaFeedReadModel {
                MediaId = @event.Id,
                FeedUrl = @event.FeedUrl
            });
        }

        public async Task On(MediaFeedRemoved @event)
        {
            var source = (await GetAll())
                .SingleOrDefault(x => x.MediaId == @event.Id && x.FeedUrl == @event.FeedUrl);

            if (source != null) {
                await Remove(source);
            }
            else {
                LogError($"Unable to apply {@event.GetType().Name} : media feed {@event.FeedUrl} on media {@event.Id} not found in projection.");
            }
        }

        public async Task On(MediaDeleted @event)
        {
            var feeds = (await GetAll()).Where(x => x.MediaId == @event.Id);
            foreach (var feed in feeds) {
                await Remove(feed);
            }
        }
    }
}