using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using NewsAggregator.Domain.Rss;

namespace NewsAggregator.Infrastructure
{
    public class RssSourceReader : IRssSourceReader
    {
        public async Task<IReadOnlyCollection<ExternalArticle>> ReadNewArticles(Uri url, DateTimeOffset? from)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url.AbsoluteUri);
            using var reader = XmlReader.Create(await response.Content.ReadAsStreamAsync());
            var feed = SyndicationFeed.Load(reader);
            return Parse(feed, from).ToArray();
        }

        private static IEnumerable<ExternalArticle> Parse(SyndicationFeed feed, DateTimeOffset? from)
        {
            var articles = feed.Items;
            if (from.HasValue) {
                articles = articles.Where(x => x.PublishDate > @from.Value);
            }
            return articles.Select(item => new ExternalArticle {
                Title = item.Title?.Text,
                Summary = item.Summary?.Text,
                PublishDate = item.PublishDate,
                Url = new Uri(item.Id)
            });
        }
    }
}