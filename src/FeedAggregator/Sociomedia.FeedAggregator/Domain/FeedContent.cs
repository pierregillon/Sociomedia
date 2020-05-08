using System.Collections.Generic;
using System.Linq;
using Sociomedia.Domain.Articles;

namespace Sociomedia.FeedAggregator.Domain
{
    public class FeedContent
    {
        public FeedContent(IReadOnlyCollection<FeedItem> items)
        {
            Items = items;
        }

        public IReadOnlyCollection<FeedItem> Items { get; }

        public IEnumerable<ExternalArticle> ToExternalArticles()
        {
            return Items.Select(item => new ExternalArticle(
                item.Id,
                item.Link,
                item.Title,
                item.PublishDate,
                item.Summary,
                item.ImageUrl
            ));
        }
    }
}