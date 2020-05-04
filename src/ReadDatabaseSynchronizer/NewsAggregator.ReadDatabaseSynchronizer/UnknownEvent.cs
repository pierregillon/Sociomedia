using System;

namespace NewsAggregator.ReadDatabaseSynchronizer {
    public class UnknownEvent : Exception
    {
        public UnknownEvent(string eventType) : base($"Event type {eventType} unknown") { }
    }
}