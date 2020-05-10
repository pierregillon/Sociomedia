using System;

namespace Sociomedia.Domain.Medias
{
    public class MediaDeleted : MediaEvent
    {
        public MediaDeleted(Guid id) : base(id) { }
    }
}