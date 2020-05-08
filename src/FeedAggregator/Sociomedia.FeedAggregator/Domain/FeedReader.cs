using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sociomedia.Domain;
using Sociomedia.Domain.Articles;
using Sociomedia.Domain.Medias;
using Sociomedia.Infrastructure;

namespace Sociomedia.FeedAggregator.Domain
{
    public class FeedReader : IFeedReader
    {
        private readonly IFeedParser _feedParser;
        private readonly IWebPageDownloader _webPageDownloader;

        public FeedReader(IFeedParser feedParser, IWebPageDownloader webPageDownloader)
        {
            _feedParser = feedParser;
            _webPageDownloader = webPageDownloader;
        }

        public async Task<IReadOnlyCollection<ExternalArticle>> ReadArticles(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));

            try {
                return await url
                    .Pipe(x => new Uri(x))
                    .Pipe(async x => await _webPageDownloader.DownloadStream(x))
                    .Pipe(x => _feedParser.Parse(x))
                    .Pipe(x => x.ToExternalArticles().ToArray());
            }
            catch (UnreachableWebDocumentException) {
                return Array.Empty<ExternalArticle>();
            }
        }
    }
}