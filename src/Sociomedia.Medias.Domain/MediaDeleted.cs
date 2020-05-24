using System;

namespace Sociomedia.Medias.Domain
{
    public class MediaDeleted : MediaEvent
    {
        public MediaDeleted(Guid id) : base(id) { }
    }
}