using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Sociomedia.Domain.Articles;

namespace Sociomedia.Infrastructure
{
    public class WebPageDownloader : IWebPageDownloader
    {
        private readonly ILogger _logger;

        public WebPageDownloader(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> Download(Uri url)
        {
            try {
                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex) {
                Error(ex, url);
                throw new UnreachableWebDocumentException(ex);
            }
        }

        public async Task<Stream> DownloadStream(Uri url)
        {
            try {
                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception ex) {
                Error(ex, url);
                throw new UnreachableWebDocumentException(ex);
            }
        }

        private void Error(Exception exception, Uri url)
        {
            _logger.Error($"[HTTP_DOWNLOADER] Unable to download web document '{url}' : {exception.Message}");
        }
    }

    public class UnreachableWebDocumentException : Exception
    {
        public UnreachableWebDocumentException(Exception innerException) : base("Web document unreachable.", innerException) { }
    }
}