using System;

namespace Sociomedia.Articles.Domain.Articles
{
    public class ArticleUpdated : ArticleEvent
    {
        public ArticleUpdated(Guid id, string title, string summary, DateTimeOffset publishDate, string url, string imageUrl) : base(id)
        {
            Title = title;
            Summary = summary;
            PublishDate = publishDate;
            Url = url;
            ImageUrl = imageUrl;
        }

        public string Title { get; }

        public string Summary { get; }

        public DateTimeOffset PublishDate { get; }

        public string Url { get; }

        public string ImageUrl { get; }
    }
}