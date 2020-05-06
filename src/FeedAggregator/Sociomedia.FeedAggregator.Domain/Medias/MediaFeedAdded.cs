using System;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public class MediaFeedAdded : DomainEvents.Media.MediaFeedAdded, IDomainEvent
    {
        public MediaFeedAdded(Guid id, string feedUrl) : base(id, feedUrl) { }
    }
}