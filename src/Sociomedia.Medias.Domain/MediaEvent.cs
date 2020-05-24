using System;
using Sociomedia.Core.Domain;

namespace Sociomedia.Medias.Domain
{
    public abstract class MediaEvent : DomainEvent
    {
        protected MediaEvent(Guid id) : base(id, "media") { }
    }
}