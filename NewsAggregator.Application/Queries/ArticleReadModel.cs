using System;

namespace NewsAggregator.Application.Queries {
    public class ArticleReadModel
    {
        public string Url { get; set; }
        public Guid RssSourceId { get; set; }
    }
}