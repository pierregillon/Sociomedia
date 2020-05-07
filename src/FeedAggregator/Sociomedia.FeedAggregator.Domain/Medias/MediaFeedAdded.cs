using System;
using CQRSlite.Events;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public class MediaFeedAdded : DomainEvents.Media.MediaFeedAdded, IEvent
    {
        public MediaFeedAdded(Guid id, string feedUrl) : base(id, feedUrl) { }
    }
}