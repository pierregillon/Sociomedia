using System;

namespace Sociomedia.Articles.Application.Queries {
    public class MediaFeedReadModel
    {
        public Guid MediaId { get; set; }
        public DateTime? LastSynchronizationDate { get; set; }
        public string FeedUrl { get; set; }
    }
}