using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sociomedia.Application.Domain;
using Sociomedia.Application.Infrastructure.EventStoring;

namespace Sociomedia.ProjectionSynchronizer.Tests
{
    public class InMemoryBus : IEventBus
    {
        private DomainEventReceived _domainEventReceived;
        private PositionInStreamChanged _positionInStreamChanged;
        public bool IsListening { get; private set; }

        public long? LastStreamPosition { get; private set; }

        public bool IsConnected => IsListening;

        public Task SubscribeToEvents(long? initialPosition, IEnumerable<Type> eventTypes, DomainEventReceived domainEventReceived, PositionInStreamChanged positionInStreamChanged = null)
        {
            _domainEventReceived = domainEventReceived;
            _positionInStreamChanged = positionInStreamChanged;
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
            await _positionInStreamChanged(position);
            await _domainEventReceived(@event);
        }
    }
}