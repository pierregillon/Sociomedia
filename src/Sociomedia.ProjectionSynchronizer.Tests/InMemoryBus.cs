using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.EventStoring;

namespace Sociomedia.ProjectionSynchronizer.Tests
{
    public class InMemoryBus : IEventBus
    {
        private DomainEventReceived _domainEventReceived;
        public bool IsListening { get; private set; }

        public long? LastStreamPosition { get; private set; }

        public bool IsConnected => IsListening;

        public Task SubscribeToEvents(long? initialPosition, IEnumerable<Type> eventTypes, DomainEventReceived domainEventReceived)
        {
            _domainEventReceived = domainEventReceived;
            LastStreamPosition = initialPosition;
            IsListening = true;
            return Task.CompletedTask;
        }

        public void Stop()
        {
            IsListening = false;
        }

        public async Task Push(long position, DomainEvent @event)
        {
            await _domainEventReceived(@event, position);
        }
    }
}