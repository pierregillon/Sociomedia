using System.Threading.Tasks;
using CQRSlite.Events;

namespace Sociomedia.Articles.Application.Projections
{
    public interface IProjection
    {
        Task On(IEvent @event);
    }
}