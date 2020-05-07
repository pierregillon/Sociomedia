using System;
using System.Collections.Generic;
using System.Linq;
using CQRSlite.Domain;
using Sociomedia.DomainEvents.Media;

namespace Sociomedia.Domain.Medias
{
    public class Media : AggregateRoot
    {
        private readonly List<string> _currentFeeds = new List<string>();

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

        public void Edit(string name, string imageUrl, PoliticalOrientation politicalOrientation)
        {
            ApplyChange(new MediaEdited(Id, name, imageUrl, politicalOrientation));
        }

        public void UpdateFeeds(IReadOnlyCollection<string> feeds)
        {
            foreach (var newFeed in feeds.Except(_currentFeeds)) {
                ApplyChange(new MediaFeedAdded(Id, newFeed));
            }

            foreach (var oldFeed in _currentFeeds.Except(feeds)) {
                ApplyChange(new MediaFeedRemoved(Id, oldFeed));
            }
        }

        private void Apply(MediaAdded @event)
        {
            Id = @event.Id;
        }

        private void Apply(MediaFeedAdded @event)
        {
            _currentFeeds.Add(@event.FeedUrl);
        }
    }
}