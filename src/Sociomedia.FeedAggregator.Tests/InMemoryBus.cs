using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.EventStoring;

namespace Sociomedia.FeedAggregator.Tests {
    public class InMemoryBus : IEventBus
    {
        private DomainEventReceived _domainEventReceived;
        private LiveProcessingStarted _liveProcessingStarted;

        public bool IsListening { get; private set; }

        public long? LastStreamPosition { get; private set; }

        public bool IsConnected => IsListening;

        public Task SubscribeToEvents(long? initialPosition, IEnumerable<Type> eventTypes, DomainEventReceived domainEventReceived, LiveProcessingStarted liveProcessingStarted)
        {
            _domainEventReceived = domainEventReceived;
            _liveProcessingStarted = liveProcessingStarted;
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

        public void SwitchToLiveMode()
        {
            _liveProcessingStarted.Invoke();
        }
    }
}