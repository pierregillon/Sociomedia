using System;

namespace NewsAggregator.Infrastructure.RSS {
    public class RssItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Summary { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
    }
}