using System;
using CQRSlite.Events;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public class MediaFeedSynchronized : DomainEvents.Media.MediaFeedSynchronized, IEvent
    {
        public MediaFeedSynchronized(Guid mediaId, string feedUrl, DateTime synchronizationDate) : base(mediaId, feedUrl, synchronizationDate) { }
    }
}