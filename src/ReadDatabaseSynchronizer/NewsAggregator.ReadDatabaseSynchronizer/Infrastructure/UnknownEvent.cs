using System;

namespace NewsAggregator.ReadDatabaseSynchronizer.Infrastructure {
    public class UnknownEvent : Exception
    {
        public UnknownEvent(string eventType) : base($"Event type {eventType} unknown") { }
    }
}