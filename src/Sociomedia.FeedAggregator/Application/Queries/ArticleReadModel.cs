using System;

namespace Sociomedia.FeedAggregator.Application.Queries {
    public class ArticleReadModel
    {
        public Guid ArticleId { get; set; }
        public string ExternalArticleId { get; set; }
        public Guid MediaId { get; set; }
        public DateTimeOffset PublishDate { get; set; }
    }
}