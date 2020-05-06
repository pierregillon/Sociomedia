using System;

namespace Sociomedia.DomainEvents.Media
{
    public class MediaFeedAdded : DomainEvent
    {
        public MediaFeedAdded(Guid id, string feedUrl) : base(id)
        {
            Id = id;
            FeedUrl = feedUrl;
        }

        public string FeedUrl { get; }
    }
}