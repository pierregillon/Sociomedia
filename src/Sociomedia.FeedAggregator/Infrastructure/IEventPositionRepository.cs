using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Infrastructure
{
    public interface IEventPositionRepository
    {
        Task<long?> GetLastProcessedPosition();
        Task Save(long position);
    }
}