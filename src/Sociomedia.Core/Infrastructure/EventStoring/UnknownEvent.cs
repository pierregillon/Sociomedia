using System;

namespace Sociomedia.Core.Infrastructure.EventStoring {
    public class UnknownEvent : Exception
    {
        public UnknownEvent(string eventType) : base($"The event type '{eventType}' is unknown.") { }
    }
}