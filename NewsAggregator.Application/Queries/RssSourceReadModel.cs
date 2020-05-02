using System;

namespace NewsAggregator.Application.Queries {
    public class RssSourceReadModel
    {
        public Guid Id { get; set; }
        public DateTime? LastSynchronizationDate { get; set; }
        public Uri Url { get; set; }
    }
}