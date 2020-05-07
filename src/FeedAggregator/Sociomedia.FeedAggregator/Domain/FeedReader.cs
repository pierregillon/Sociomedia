using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Sociomedia.Domain;
using Sociomedia.Domain.Articles;
using Sociomedia.Domain.Medias;

namespace Sociomedia.FeedAggregator.Domain
{
    public class FeedReader : IFeedReader
    {
        private readonly IFeedParser _feedParser;
        private readonly ILogger _logger;

        public FeedReader(IFeedParser feedParser, ILogger logger)
        {
            _feedParser = feedParser;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<ExternalArticle>> ReadNewArticles(string url, DateTimeOffset? from)
        {
            var response = await Download(url);
            if (response == null) {
                return Array.Empty<ExternalArticle>();
            }
            return await response
                .Pipe(async x => await x.Content.ReadAsStreamAsync())
                .Pipe(x => _feedParser.Parse(x))
                .Pipe(x => x.ToExternalArticles(from).ToArray());
        }

        private async Task<HttpResponseMessage> Download(string url)
        {
            try {
                using var client = new HttpClient();
                return await client.GetAsync(url);
            }
            catch (Exception ex) {
                _logger.Error($"[HTTP_DOWNLOADER] Unable to read feed '{url}' : {ex.Message}");
                return null;
            }
        }
    }
}