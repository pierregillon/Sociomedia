using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Sociomedia.FeedAggregator.Domain;
using Sociomedia.FeedAggregator.Domain.Rss;

namespace Sociomedia.FeedAggregator.Infrastructure.RSS
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
            return await url
                .Pipe(Download)
                .Pipe(async x => await x.Content.ReadAsStreamAsync())
                .Pipe(x => _rssParser.Parse(x))
                .Pipe(x => x.ToExternalArticles(from).ToArray());
        }

        private static async Task<HttpResponseMessage> Download(Uri url)
        {
            using var client = new HttpClient();
            return await client.GetAsync(url.AbsoluteUri);
        }
    }
}