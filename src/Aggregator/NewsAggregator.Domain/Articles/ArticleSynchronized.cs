using System;
using System.Collections.Generic;

namespace NewsAggregator.Domain.Articles
{
    public class ArticleSynchronized : DomainEvent
    {
        public ArticleSynchronized(Guid id, string title, string summary, DateTimeOffset publishDate, Uri url, Uri imageUrl, IReadOnlyCollection<string> keywords, Guid rssSourceId)
        {
            Id = id;
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
        public Uri Url { get; }
        public Uri ImageUrl { get; }
        public Guid RssSourceId { get; }
    }
}