using System;

namespace NewsAggregator.Front.Data
{
    public class ArticleListItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string WebSiteSource => new Uri(Url).Host;
    }
}
