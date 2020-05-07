using System;

namespace Sociomedia.Domain.Medias
{
    public abstract class MediaEvent : DomainEvent
    {
        protected MediaEvent(Guid id) : base(id, "media") { }
    }
}