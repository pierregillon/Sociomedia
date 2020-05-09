using System.Threading.Tasks;
using Sociomedia.Domain;

namespace Sociomedia.Application
{
    public interface IEventListener<in T> where T : DomainEvent
    {
        Task On(T @event);
    }
}