using System;

namespace Sociomedia.FeedAggregator.Infrastructure {
    public class UnknownEvent : Exception
    {
        public UnknownEvent(string eventType) : base($"The event type '{eventType}' is unknown.") { }
    }
}