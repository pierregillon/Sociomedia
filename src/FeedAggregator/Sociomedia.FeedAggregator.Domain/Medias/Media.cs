using System;
using CQRSlite.Domain;
using Sociomedia.DomainEvents.RssSource;

namespace Sociomedia.FeedAggregator.Domain.Medias {
    public class Media : AggregateRoot
    {
        public Media(string name, string imageUrl, PoliticalOrientation politicalOrientation)
        {
            this.ApplyChange(new MediaAdded(Guid.NewGuid(), name, imageUrl, politicalOrientation));
        }

        private void Apply(MediaAdded @event)
        {
            Id = @event.Id;
        }
    }
}