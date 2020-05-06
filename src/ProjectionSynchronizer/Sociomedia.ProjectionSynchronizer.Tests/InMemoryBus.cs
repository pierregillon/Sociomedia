using System;
using System.Threading.Tasks;
using Sociomedia.DomainEvents;
using Sociomedia.ProjectionSynchronizer.Application;

namespace Sociomedia.ProjectionSynchronizer.Tests {
    public class InMemoryBus : IEventBus
    {
        private DomainEventReceived _domainEventReceived;

        public long? LastStreamPosition { get; private set; }

        public Task StartListeningEvents(long? lastPosition, DomainEventReceived domainEventReceived)
        {
            _domainEventReceived = domainEventReceived;
            LastStreamPosition = lastPosition;
            return Task.CompletedTask;
        }

        public void StopListeningEvents()
        {
            throw new NotImplementedException();
        }

        public async Task Push(long position, DomainEvent @event)
        {
            await _domainEventReceived(position, @event);
        }
    }
}