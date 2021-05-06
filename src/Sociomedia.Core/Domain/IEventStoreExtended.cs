using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.Client;

namespace Sociomedia.Core.Domain
{
    public interface IEventStoreExtended
    {
        IAsyncEnumerable<IEvent> GetAllEventsBetween(Position startPosition, Position endPosition, IReadOnlyCollection<Type> eventTypes);
        Task<long> GetCurrentGlobalStreamPosition();
    }
}