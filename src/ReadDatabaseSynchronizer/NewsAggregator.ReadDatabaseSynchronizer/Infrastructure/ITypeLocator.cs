using System;

namespace NewsAggregator.ReadDatabaseSynchronizer.Infrastructure {
    public interface ITypeLocator
    {
        Type FindEventType(string eventType);
    }
}