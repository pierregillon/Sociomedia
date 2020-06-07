using System.Threading.Tasks;
using CQRSlite.Events;

namespace Sociomedia.Core.Application.Projections
{
    public interface IProjection
    {
        Task On(IEvent @event);
    }
}