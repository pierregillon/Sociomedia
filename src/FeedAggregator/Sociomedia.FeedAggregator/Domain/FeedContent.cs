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

        public IEnumerable<ExternalArticle> ToExternalArticles()
        {
            return Items.Select(item => new ExternalArticle {
                Id = item.Id,
                Title = item.Title,
                Summary = item.Summary,
                PublishDate = item.PublishDate,
                Url = new Uri(item.Link),
                ImageUrl = string.IsNullOrEmpty(item.ImageUrl) ? null : new Uri(item.ImageUrl)
            });
        }
    }
}