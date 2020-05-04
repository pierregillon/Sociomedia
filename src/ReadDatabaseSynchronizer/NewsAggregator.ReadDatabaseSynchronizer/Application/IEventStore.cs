using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace NewsAggregator.ReadDatabaseSynchronizer.Application {
    public interface IEventStore
    {
        Task Connect(string server, int port = 1113, string login = "admin", string password = "changeit");
        void StartListeningEvents(Position? position, DomainEventReceived domainEventReceived);
        void StopListeningEvents();
    }
}