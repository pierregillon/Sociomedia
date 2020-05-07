using System;
using System.Net.Http;
using System.Threading.Tasks;
using Sociomedia.Domain.Articles;

namespace Sociomedia.Infrastructure {
    public class HtmlPageDownloader : IHtmlPageDownloader
    {
        public async Task<string> Download(Uri url)
        {
            try {
                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex) {
                return null;
            }
        }
    }
}