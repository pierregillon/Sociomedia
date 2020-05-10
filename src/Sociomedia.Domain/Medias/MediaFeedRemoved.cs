using System;
using CQRSlite.Events;

namespace Sociomedia.Domain.Medias
{
    public class MediaFeedRemoved : MediaEvent
    {
        public MediaFeedRemoved(Guid mediaId, string feedUrl) : base(mediaId)
        {
            FeedUrl = feedUrl;
        }

        public string FeedUrl { get; }
    }
}