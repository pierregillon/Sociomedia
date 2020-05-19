using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sociomedia.Articles.Domain;
using Sociomedia.Core.Domain;

namespace Sociomedia.Articles.Infrastructure
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

        public async Task<IReadOnlyCollection<FeedItem>> Read(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));

            try {
                return await url
                    .Pipe(_webPageDownloader.DownloadStream)
                    .Pipe(_feedParser.Parse);
            }
            catch (UnreachableWebDocumentException) {
                return Array.Empty<FeedItem>();
            }
        }
    }
}