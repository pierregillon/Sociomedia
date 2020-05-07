using System;

namespace Sociomedia.DomainEvents.Media
{
    public abstract class MediaEvent : DomainEvent
    {
        protected MediaEvent(Guid id) : base(id, "media") { }
    }
}