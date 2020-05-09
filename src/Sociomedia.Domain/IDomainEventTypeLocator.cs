using System;

namespace Sociomedia.Domain
{
    public interface IDomainEventTypeLocator
    {
        Type FindEventType(string eventType);
    }
}