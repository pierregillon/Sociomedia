using System;

namespace NewsAggregator.ReadDatabaseSynchronizer {
    public interface ITypeLocator
    {
        Type FindEventType(string eventType);
    }
}