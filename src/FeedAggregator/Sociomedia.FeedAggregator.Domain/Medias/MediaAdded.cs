using System;
using CQRSlite.Events;
using Sociomedia.DomainEvents.Media;

namespace Sociomedia.Domain.Medias
{
    public class MediaAdded : DomainEvents.Media.MediaAdded, IEvent
    {
        public MediaAdded(Guid id, string name, string imageUrl, PoliticalOrientation politicalOrientation) : base(id, name, imageUrl, politicalOrientation) { }
    }
}