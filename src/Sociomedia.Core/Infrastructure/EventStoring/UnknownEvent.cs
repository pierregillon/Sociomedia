using System;

namespace Sociomedia.Application.Infrastructure.EventStoring {
    public class UnknownEvent : Exception
    {
        public UnknownEvent(string eventType) : base($"The event type '{eventType}' is unknown.") { }
    }
}