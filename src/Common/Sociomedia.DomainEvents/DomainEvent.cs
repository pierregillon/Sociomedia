using System;

namespace Sociomedia.DomainEvents
{
    public abstract class DomainEvent
    {
        protected DomainEvent(Guid id, string category)
        {
            Id = id;
            Category = category;
        }

        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string Category { get; }

        public string EventStream => Category + "-" + Id;
    }
}