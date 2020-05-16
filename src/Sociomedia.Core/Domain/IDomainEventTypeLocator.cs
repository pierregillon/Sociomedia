using System;

namespace Sociomedia.Application.Domain
{
    public interface IDomainEventTypeLocator
    {
        Type FindEventType(string eventType);
    }
}