using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Application.Queries
{
    public class MediaFeedFinder : IMediaFeedFinder
    {
        private readonly InMemoryDatabase _database;

        public MediaFeedFinder(InMemoryDatabase database)
        {
            _database = database;
        }

        public Task<IReadOnlyCollection<MediaFeedReadModel>> GetAll()
        {
            return Task.FromResult((IReadOnlyCollection<MediaFeedReadModel>) _database.List<MediaFeedReadModel>().ToArray());
        }
    }
}