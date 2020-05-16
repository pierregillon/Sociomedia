using System.Threading.Tasks;
using CQRSlite.Events;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public delegate Task DomainEventReceived(IEvent @event);
}