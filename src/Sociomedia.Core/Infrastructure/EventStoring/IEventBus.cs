using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public interface IEventBus
    {
        bool IsConnected { get; }
        Task SubscribeToEvents(long? initialPosition, IEnumerable<Type> eventTypes, DomainEventReceived domainEventReceived, LiveProcessingStarted liveProcessingStarted = null);
        void Stop();
    }
}