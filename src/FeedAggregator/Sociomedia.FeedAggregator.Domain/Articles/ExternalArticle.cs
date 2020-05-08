using System;

namespace Sociomedia.Domain.Articles
{
    public class ExternalArticle
    {
        public string Id { get; set; }
        public Uri Url { get; set; }
        public string Title { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Summary { get; set; }
        public Uri ImageUrl { get; set; }
    }
}