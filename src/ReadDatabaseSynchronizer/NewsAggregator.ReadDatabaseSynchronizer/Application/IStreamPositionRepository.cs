using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace NewsAggregator.ReadDatabaseSynchronizer.Application {
    public interface IStreamPositionRepository
    {
        Task Save(Position position);
        Task<Position?> GetLastPosition();
    }
}