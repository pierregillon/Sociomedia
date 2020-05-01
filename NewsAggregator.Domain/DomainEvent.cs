using System;

namespace NewsAggregator.Domain {
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}