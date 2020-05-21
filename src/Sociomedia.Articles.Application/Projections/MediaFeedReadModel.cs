using System;

namespace Sociomedia.Articles.Application.Projections {
    public class MediaFeedReadModel
    {
        public Guid MediaId { get; set; }
        public DateTime? LastSynchronizationDate { get; set; }
        public string FeedUrl { get; set; }
    }
}