using System;

namespace Sociomedia.ProjectionSynchronizer.Infrastructure {
    public interface ITypeLocator
    {
        Type FindEventType(string eventType);
    }
}