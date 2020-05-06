using System.Threading.Tasks;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public interface IEventBus
    {
        Task StartListeningEvents(long? lastPosition, DomainEventReceived domainEventReceived);
        void StopListeningEvents();
    }
}