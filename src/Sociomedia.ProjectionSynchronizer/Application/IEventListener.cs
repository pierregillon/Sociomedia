using System.Threading.Tasks;
using Sociomedia.Domain;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public interface IEventListener<in T> where T : DomainEvent
    {
        Task On(T @event);
    }
}