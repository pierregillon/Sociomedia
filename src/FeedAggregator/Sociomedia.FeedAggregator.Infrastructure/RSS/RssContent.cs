using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.FeedAggregator.Domain.Rss;

namespace Sociomedia.FeedAggregator.Infrastructure.RSS
{
    public class RssContent
    {
        public RssContent(IReadOnlyCollection<RssItem> items)
        {
            Items = items;
        }

        public IReadOnlyCollection<RssItem> Items { get; }

        public IEnumerable<ExternalArticle> ToExternalArticles(DateTimeOffset? @from)
        {
            IEnumerable<RssItem> articles = Items;
            if (from.HasValue) {
                articles = articles.Where(x => x.PublishDate > @from.Value);
            }
            return articles.Select(item => new ExternalArticle {
                Title = item.Title,
                Summary = item.Summary,
                PublishDate = item.PublishDate,
                Url = new Uri(item.Link),
                ImageUrl = string.IsNullOrEmpty(item.ImageUrl) ? null : new Uri(item.ImageUrl)
            });
        }
    }
}