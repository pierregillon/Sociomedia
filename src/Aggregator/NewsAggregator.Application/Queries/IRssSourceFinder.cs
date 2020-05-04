using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsAggregator.Application.Queries {
    public interface IRssSourceFinder
    {
        Task<IReadOnlyCollection<RssSourceReadModel>> GetAll();
    }
}