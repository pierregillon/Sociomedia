using System;
using Sociomedia.DomainEvents.Media;

namespace Sociomedia.FeedAggregator.Domain.Medias {
    public class MediaEdited : DomainEvents.Media.MediaEdited, IDomainEvent
    {
        public MediaEdited(Guid id, string name, string imageUrl, PoliticalOrientation politicalOrientation) : base(id, name, imageUrl, politicalOrientation) { }
    }
}