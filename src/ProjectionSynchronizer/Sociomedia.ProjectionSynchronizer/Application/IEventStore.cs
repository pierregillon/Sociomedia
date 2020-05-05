using System.Threading.Tasks;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public interface IEventStore
    {
        Task Connect(string server, int port = 1113, string login = "admin", string password = "changeit");
        void StartListeningEvents(long? lastPosition, DomainEventReceived domainEventReceived);
        void StopListeningEvents();
    }
}