using System.Threading.Tasks;
using Sociomedia.DomainEvents;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public delegate Task DomainEventReceived(long position, DomainEvent @event);
}