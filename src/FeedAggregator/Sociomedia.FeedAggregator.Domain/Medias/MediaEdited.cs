using System;
using CQRSlite.Events;
using Sociomedia.DomainEvents.Media;

namespace Sociomedia.Domain.Medias {
    public class MediaEdited : DomainEvents.Media.MediaEdited, IEvent
    {
        public MediaEdited(Guid id, string name, string imageUrl, PoliticalOrientation politicalOrientation) : base(id, name, imageUrl, politicalOrientation) { }
    }
}