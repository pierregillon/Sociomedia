using System.Threading.Tasks;

namespace Sociomedia.Core.Application
{
    public interface IEventPositionRepository
    {
        Task<long?> GetLastProcessedPosition();
        Task Save(long position);
    }
}