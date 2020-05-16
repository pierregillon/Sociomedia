using System.Threading.Tasks;
using Sociomedia.Application.Domain;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public delegate Task DomainEventReceived(long position, DomainEvent @event);
}