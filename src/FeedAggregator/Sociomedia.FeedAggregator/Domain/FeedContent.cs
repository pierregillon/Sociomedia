using System;
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

        public IEnumerable<ExternalArticle> ToExternalArticles(DateTimeOffset? @from)
        {
            IEnumerable<FeedItem> articles = Items;
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