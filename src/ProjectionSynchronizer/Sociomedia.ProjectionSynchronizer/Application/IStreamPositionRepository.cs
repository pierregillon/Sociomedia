using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Sociomedia.ProjectionSynchronizer.Application {
    public interface IStreamPositionRepository
    {
        Task Save(Position position);
        Task<Position?> GetLastPosition();
    }
}