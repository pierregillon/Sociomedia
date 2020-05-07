using System.Threading.Tasks;
using Sociomedia.Domain;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public delegate Task DomainEventReceived(long position, DomainEvent @event);
}