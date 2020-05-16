using System.Threading.Tasks;
using CQRSlite.Events;

namespace Sociomedia.Application.Infrastructure.EventStoring
{
    public delegate Task DomainEventReceived(IEvent @event);
}