using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Sociomedia.Domain;
using Sociomedia.Domain.Articles;
using Sociomedia.Domain.Medias;

namespace Sociomedia.FeedAggregator.Domain
{
    public class FeedReader : IFeedReader
    {
        private readonly IFeedParser _feedParser;

        public FeedReader(IFeedParser feedParser)
        {
            _feedParser = feedParser;
        }

        public async Task<IReadOnlyCollection<ExternalArticle>> ReadNewArticles(string url, DateTimeOffset? from)
        {
            return await url
                .Pipe(Download)
                .Pipe(async x => await x.Content.ReadAsStreamAsync())
                .Pipe(x => _feedParser.Parse(x))
                .Pipe(x => x.ToExternalArticles(from).ToArray());
        }

        private static async Task<HttpResponseMessage> Download(string url)
        {
            using var client = new HttpClient();
            return await client.GetAsync(url);
        }
    }
}