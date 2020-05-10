using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sociomedia.Domain.Articles
{
    public class ArticleFactory
    {
        private readonly IHtmlParser _htmlParser;
        private readonly IWebPageDownloader _webPageDownloader;

        public ArticleFactory(IHtmlParser htmlParser, IWebPageDownloader webPageDownloader)
        {
            _htmlParser = htmlParser;
            _webPageDownloader = webPageDownloader;
        }

        public async Task<Article> Build(Guid mediaId, ExternalArticle externalArticle)
        {
            var html = await _webPageDownloader.Download(externalArticle.Url);

            var articleContent = _htmlParser.ExtractPlainTextArticleContent(html);

            externalArticle.ImageUrl ??= html
                .Pipe(_htmlParser.ExtractArticleImageUrl)
                .Pipe(imageUrl => InjectHostIfMissing(imageUrl, externalArticle.Url))
                .Pipe(UrlSanitizer.Sanitize);

            var keywords = new KeywordsParser().Parse(articleContent).Take(50).ToArray();

            return new Article(mediaId, externalArticle, keywords.Select(x => x.ToString()).ToArray());
        }

        private static string InjectHostIfMissing(string imageUrl, string articleUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) {
                return imageUrl;
            }
            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out var absoluteImageUrl)) {
                return absoluteImageUrl.AbsoluteUri;
            }
            var articleUri = new Uri(articleUrl);
            var schemeAndHost = new Uri($"{articleUri.Scheme}://{articleUri.Host}");
            return new Uri(schemeAndHost, imageUrl).AbsoluteUri;
        }
    }
}