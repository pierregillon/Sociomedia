using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NewsAggregator.Domain.Rss;

namespace NewsAggregator.Infrastructure.RSS
{
    public class RssSourceReader : IRssSourceReader
    {
        private readonly IRssParser _rssParser;

        public RssSourceReader(IRssParser rssParser)
        {
            _rssParser = rssParser;
        }

        public async Task<IReadOnlyCollection<ExternalArticle>> ReadNewArticles(Uri url, DateTimeOffset? from)
        {
            var response = await Download(url);
            var rssContent = _rssParser.Parse(await response.Content.ReadAsStreamAsync());
            return rssContent.ToExternalArticles(from).ToArray();
        }

        private static async Task<HttpResponseMessage> Download(Uri url)
        {
            using var client = new HttpClient();
            return await client.GetAsync(url.AbsoluteUri);
        }
    }
}