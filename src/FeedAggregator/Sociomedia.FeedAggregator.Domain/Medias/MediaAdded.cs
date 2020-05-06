using System;
using CQRSlite.Events;
using Sociomedia.DomainEvents.RssSource;

namespace Sociomedia.FeedAggregator.Domain.Medias {
    public class MediaAdded : DomainEvents.RssSource.MediaAdded, IDomainEvent
    {
        public MediaAdded(Guid id, string name, string imageUrl, PoliticalOrientation politicalOrientation) : base(id, name, imageUrl, politicalOrientation) { }
    }
}