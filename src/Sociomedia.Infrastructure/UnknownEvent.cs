using System;

namespace Sociomedia.Infrastructure {
    public class UnknownEvent : Exception
    {
        public UnknownEvent(string eventType) : base($"The event type '{eventType}' is unknown.") { }
    }
}