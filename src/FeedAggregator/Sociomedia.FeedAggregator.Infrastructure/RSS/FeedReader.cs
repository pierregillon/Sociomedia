using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Sociomedia.FeedAggregator.Domain;
using Sociomedia.FeedAggregator.Domain.Medias;

namespace Sociomedia.FeedAggregator.Infrastructure.RSS
{
    public class FeedReader : IFeedReader
    {
        private readonly IRssParser _rssParser;

        public FeedReader(IRssParser rssParser)
        {
            _rssParser = rssParser;
        }

        public async Task<IReadOnlyCollection<ExternalArticle>> ReadNewArticles(string url, DateTimeOffset? from)
        {
            return await url
                .Pipe(Download)
                .Pipe(async x => await x.Content.ReadAsStreamAsync())
                .Pipe(x => _rssParser.Parse(x))
                .Pipe(x => x.ToExternalArticles(from).ToArray());
        }

        private static async Task<HttpResponseMessage> Download(string url)
        {
            using var client = new HttpClient();
            return await client.GetAsync(url);
        }
    }
}