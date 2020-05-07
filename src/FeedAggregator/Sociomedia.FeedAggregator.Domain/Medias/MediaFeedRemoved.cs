using System;
using CQRSlite.Events;

namespace Sociomedia.Domain.Medias
{
    public class MediaFeedRemoved : DomainEvents.Media.MediaFeedRemoved, IEvent
    {
        public MediaFeedRemoved(Guid mediaId, string feedUrl) : base(mediaId, feedUrl) { }
    }
}