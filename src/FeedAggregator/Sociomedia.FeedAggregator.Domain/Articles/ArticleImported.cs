using System;
using System.Collections.Generic;

namespace Sociomedia.Domain.Articles
{
    public class ArticleImported : ArticleEvent
    {
        public ArticleImported(Guid id, string title, string summary, DateTimeOffset publishDate, string url, string imageUrl, IReadOnlyCollection<string> keywords, Guid mediaId) : base(id)
        {
            Title = title;
            Summary = summary;
            PublishDate = publishDate;
            Url = url;
            ImageUrl = imageUrl;
            Keywords = keywords;
            MediaId = mediaId;
        }

        public string Title { get; }
        public string Summary { get; }
        public DateTimeOffset PublishDate { get; }
        public IReadOnlyCollection<string> Keywords { get; }
        public string Url { get; set; }
        public string ImageUrl { get; }
        public Guid MediaId { get; }
    }
}