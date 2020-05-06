using System;

namespace Sociomedia.DomainEvents
{
    public interface IDomainEventTypeLocator
    {
        Type FindEventType(string eventType);
    }
}