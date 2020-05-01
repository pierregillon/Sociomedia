using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsAggregator.Application.Queries {
    public class RssSourceFinder : IRssSourceFinder
    {
        private readonly ReadModelDatabase _database;

        public RssSourceFinder(ReadModelDatabase database)
        {
            _database = database;
        }

        public Task<IReadOnlyCollection<RssSourceReadModel>> GetAll()
        {
            return Task.FromResult((IReadOnlyCollection<RssSourceReadModel>) _database.Sources.ToArray());
        }
    }
}