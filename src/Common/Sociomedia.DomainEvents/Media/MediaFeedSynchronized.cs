using System;

namespace Sociomedia.DomainEvents.Media
{
    public class MediaFeedSynchronized : DomainEvent
    {
        public string FeedUrl { get; }
        public DateTime SynchronizationDate { get; }

        public MediaFeedSynchronized(Guid mediaId, string feedUrl, DateTime synchronizationDate) : base(mediaId)
        {
            FeedUrl = feedUrl;
            SynchronizationDate = synchronizationDate;
        }
    }
}