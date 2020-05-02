using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var articles = ParseArticles(feed);
            if (from.HasValue) {
                articles = articles.Where(x => x.PublishDate > @from.Value);
            }
            return articles;
        }

        private static IEnumerable<ExternalArticle> ParseArticles(SyndicationFeed feed)
        {
            foreach (var externalArticle in feed.Items) {
                var article = ParseArticle(externalArticle);
                if (article != null) {
                    yield return article;
                }
            }
        }

        private static ExternalArticle ParseArticle(SyndicationItem article)
        {
            try {
                return new ExternalArticle {
                    Title = WebUtility.HtmlDecode(article.Title?.Text),
                    Summary = WebUtility.HtmlDecode(article.Summary?.Text),
                    PublishDate = ExtractPublishDate(article),
                    Url = new Uri(article.Id)
                };
            }
            catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }
        }

        private static DateTimeOffset ExtractPublishDate(SyndicationItem article)
        {
            try {
                return article.PublishDate;
            }
            catch (Exception ex) {
                return DateTimeOffset.Now;
            }
        }
    }
}