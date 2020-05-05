using System;

namespace Sociomedia.ProjectionSynchronizer.Infrastructure {
    public class UnknownEvent : Exception
    {
        public UnknownEvent(string eventType) : base($"Event type {eventType} unknown") { }
    }
}