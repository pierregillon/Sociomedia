using System.Threading.Tasks;

namespace Sociomedia.ProjectionSynchronizer.Application {
    public interface IStreamPositionRepository
    {
        Task Save(long position);
        Task<long?> GetLastPosition();
    }
}