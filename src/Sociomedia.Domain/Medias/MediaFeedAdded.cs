using System;
using CQRSlite.Events;

namespace Sociomedia.Domain.Medias
{
    public class MediaFeedAdded : MediaEvent
    {
        public MediaFeedAdded(Guid id, string feedUrl) : base(id)
        {
            Id = id;
            FeedUrl = feedUrl;
        }

        public string FeedUrl { get; }
    }
}