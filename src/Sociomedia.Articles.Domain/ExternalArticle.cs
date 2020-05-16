using System;

namespace Sociomedia.Articles.Domain
{
    public class ExternalArticle
    {
        public ExternalArticle(string id, string url, string title, DateTimeOffset publishDate, string summary, string imageUrl)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Id = id ?? url;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            PublishDate = publishDate;
            Summary = summary;
            ImageUrl = imageUrl;
        }

        public string Id { get; }
        public string Url { get; }
        public string Title { get; }
        public DateTimeOffset PublishDate { get; }
        public string Summary { get; }
        public string ImageUrl { get; set; }
    }
}