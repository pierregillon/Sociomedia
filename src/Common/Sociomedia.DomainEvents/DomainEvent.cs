using System;

namespace Sociomedia.DomainEvents
{
    public abstract class DomainEvent
    {
        protected DomainEvent(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}