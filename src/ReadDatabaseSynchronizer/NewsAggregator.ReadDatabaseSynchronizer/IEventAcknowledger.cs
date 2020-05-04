using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace NewsAggregator.ReadDatabaseSynchronizer {
    public interface IEventAcknowledger
    {
        Task Acknowledge(Position position);
        Task<Position?> GetLastPosition();
    }
}