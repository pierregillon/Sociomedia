using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Application.Queries {
    public interface IRssSourceFinder
    {
        Task<IReadOnlyCollection<RssSourceReadModel>> GetAll();
    }
}