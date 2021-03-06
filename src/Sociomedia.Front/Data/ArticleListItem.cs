using System;

namespace Sociomedia.Front.Data
{
    public class ArticleListItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string MediaImageUrl { get; set; }
        public Guid MediaId { get; set; }
    }
}
