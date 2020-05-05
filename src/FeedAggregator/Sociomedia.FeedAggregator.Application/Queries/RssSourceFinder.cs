using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Application.Queries
{
    public class RssSourceFinder : IRssSourceFinder
    {
        private readonly InMemoryDatabase _database;

        public RssSourceFinder(InMemoryDatabase database)
        {
            _database = database;
        }

        public Task<IReadOnlyCollection<RssSourceReadModel>> GetAll()
        {
            return Task.FromResult((IReadOnlyCollection<RssSourceReadModel>) _database.List<RssSourceReadModel>().ToArray());
        }
    }
}