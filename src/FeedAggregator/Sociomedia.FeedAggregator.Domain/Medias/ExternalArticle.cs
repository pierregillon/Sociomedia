using System;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public class ExternalArticle
    {
        public Uri Url { get; set; }
        public string Title { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Summary { get; set; }
        public Uri ImageUrl { get; set; }
    }
}