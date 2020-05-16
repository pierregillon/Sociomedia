using System;

namespace Sociomedia.Core.Domain
{
    public interface IDomainEventTypeLocator
    {
        Type FindEventType(string eventType);
    }
}