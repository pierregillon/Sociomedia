using System;

namespace Sociomedia.Domain.Medias
{
    public class MediaFeedSynchronized : MediaEvent
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