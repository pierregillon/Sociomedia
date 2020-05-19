using System.Threading.Tasks;
using Sociomedia.Articles.Application.Commands.SynchronizeMediaFeeds;
using Sociomedia.Core.Application;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Medias.Domain;

namespace Sociomedia.Articles.Application
{
    public class SynchronizeFeedRss : IEventListener<MediaFeedAdded>
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public SynchronizeFeedRss(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public async Task On(MediaFeedAdded @event)
        {
            await _commandDispatcher.Dispatch(new SynchronizeMediaFeedCommand(@event.Id, @event.FeedUrl));
        }
    }
}