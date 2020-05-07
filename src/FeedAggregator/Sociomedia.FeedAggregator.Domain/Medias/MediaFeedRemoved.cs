using System;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public class MediaFeedRemoved : DomainEvents.Media.MediaFeedRemoved, IDomainEvent
    {
        public MediaFeedRemoved(Guid mediaId, string feedUrl) : base(mediaId, feedUrl) { }
    }
}