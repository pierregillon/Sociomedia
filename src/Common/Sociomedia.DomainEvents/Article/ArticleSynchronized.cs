using System;
using System.Collections.Generic;

namespace Sociomedia.DomainEvents.Article
{
    public class ArticleSynchronized : DomainEvent
    {
        public ArticleSynchronized(Guid id, string title, string summary, DateTimeOffset publishDate, string url, string imageUrl, IReadOnlyCollection<string> keywords, Guid rssSourceId) : base(id)
        {
            Title = title;
            Summary = summary;
            PublishDate = publishDate;
            Url = url;
            ImageUrl = imageUrl;
            Keywords = keywords;
            RssSourceId = rssSourceId;
        }

        public string Title { get; }
        public string Summary { get; }
        public DateTimeOffset PublishDate { get; }
        public IReadOnlyCollection<string> Keywords { get; }
        public string Url { get; set; }
        public string ImageUrl { get; }
        public Guid RssSourceId { get; }
    }
}