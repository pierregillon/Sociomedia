using System;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public class MediaFeedSynchronized : DomainEvents.Media.MediaFeedSynchronized, IDomainEvent
    {
        public MediaFeedSynchronized(Guid mediaId, string feedUrl, DateTime synchronizationDate) : base(mediaId, feedUrl, synchronizationDate) { }
    }
}