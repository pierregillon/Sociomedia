using System;
using Sociomedia.DomainEvents.Media;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public class MediaAdded : DomainEvents.Media.MediaAdded, IDomainEvent
    {
        public MediaAdded(Guid id, string name, string imageUrl, PoliticalOrientation politicalOrientation) : base(id, name, imageUrl, politicalOrientation) { }
    }
}