using System;
using System.Threading.Tasks;
using Sociomedia.Application.Domain;
using Sociomedia.ProjectionSynchronizer.Application;

namespace Sociomedia.ProjectionSynchronizer.Tests
{
    public class InMemoryBus : IEventBus
    {
        private DomainEventReceived _domainEventReceived;
        private Func<Task> _disconnected;
        public bool IsListening { get; private set; }

        public long? LastStreamPosition { get; private set; }

        public Task StartListeningEvents(long? lastPosition, DomainEventReceived domainEventReceived, Func<Task> disconnected)
        {
            _domainEventReceived = domainEventReceived;
            LastStreamPosition = lastPosition;
            _disconnected = disconnected;
            IsListening = true;
            return Task.CompletedTask;
        }

        public void StopListeningEvents()
        {
            IsListening = false;
        }

        public async Task Push(long position, DomainEvent @event)
        {
            await _domainEventReceived(position, @event);
        }

        public Task SimulateConnectionLost()
        {
            return _disconnected.Invoke();
        }
    }
}