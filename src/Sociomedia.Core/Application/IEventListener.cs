using System.Threading.Tasks;
using Sociomedia.Core.Domain;

namespace Sociomedia.Core.Application
{
    public interface IEventListener<in T>
    {
        Task On(T @event);
    }
}