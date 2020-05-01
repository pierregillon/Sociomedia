using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsAggregator.Domain.Rss
{
    public interface IRssSourceRepository
    {
        Task<IReadOnlyCollection<RssSource>> GetAll();
        Task Save(RssSource rssSource);
    }
}