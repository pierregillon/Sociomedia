using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using NewsAggregator.Domain.Rss;

namespace NewsAggregator.Infrastructure
{
    public class RssFeedReader : IRssFeedReader
    {
        public async Task<RssFeeds> ReadNewFeeds(Uri url, DateTime? from)
        {
            await Task.Delay(0);

            var reader = XmlReader.Create(url.AbsoluteUri);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();
            return new RssFeeds(Parse(feed, from));
        }

        private static IEnumerable<RssFeed> Parse(SyndicationFeed feed, DateTime? from)
        {
            var feeds = feed.Items;
            if (from.HasValue) {
                feeds = feeds.Where(x => x.PublishDate > @from.Value);
            }
            return feeds.Select(item => new RssFeed {
                Title = item.Title?.Text,
                Summary = item.Summary?.Text,
                PublishDate = item.PublishDate,
                Url = new Uri(item.Id)
            });
        }
    }
}