using System;
using CQRSlite.Domain;
using Sociomedia.DomainEvents.Media;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public class Media : AggregateRoot
    {
        private Media() { }

        public Media(string name, string imageUrl, PoliticalOrientation politicalOrientation)
        {
            ApplyChange(new MediaAdded(Guid.NewGuid(), name, imageUrl, politicalOrientation));
        }

        public void AddFeed(string feedUrl)
        {
            ApplyChange(new MediaFeedAdded(Id, feedUrl));
        }

        public void SynchronizeFeed(string feedUrl)
        {
            ApplyChange(new MediaFeedSynchronized(Id, feedUrl, DateTime.UtcNow));
        }

        private void Apply(MediaAdded @event)
        {
            Id = @event.Id;
        }
    }
}