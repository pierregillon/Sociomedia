using System.Threading.Tasks;
using Sociomedia.Application.Domain;

namespace Sociomedia.Application.Application
{
    public interface IEventListener<in T> where T : DomainEvent
    {
        Task On(T @event);
    }
}