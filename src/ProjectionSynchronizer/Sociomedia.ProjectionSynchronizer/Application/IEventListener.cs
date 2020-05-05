using System.Threading.Tasks;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public interface IEventListener<T> where T : IDomainEvent
    {
        Task On(T @event);
    }
}