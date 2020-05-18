using System;
using System.Linq;
using System.Threading.Tasks;
using Sociomedia.Core.Domain;

namespace Sociomedia.Articles.Domain
{
    public class ArticleFactory
    {
        private readonly IHtmlParser _htmlParser;
        private readonly IWebPageDownloader _webPageDownloader;
        private readonly KeywordsParser _keywordsParser;

        public ArticleFactory(IHtmlParser htmlParser, IWebPageDownloader webPageDownloader, KeywordsParser keywordsParser)
        {
            _htmlParser = htmlParser;
            _webPageDownloader = webPageDownloader;
            _keywordsParser = keywordsParser;
        }

        public async Task<Article> Build(Guid mediaId, ExternalArticle externalArticle)
        {
            var html = await _webPageDownloader.Download(externalArticle.Url);

            var articleContent = _htmlParser.ExtractPlainTextArticleContent(html);

            externalArticle.ImageUrl ??= html
                .Pipe(_htmlParser.ExtractArticleImageUrl)
                .Pipe(imageUrl => InjectHostIfMissing(imageUrl, externalArticle.Url))
                .Pipe(UrlSanitizer.Sanitize);

            var text = string.Join(" ", new[] {
                articleContent,
                externalArticle.Title,
                externalArticle.Summary
            });

            var keywords = _keywordsParser.Parse(text).Take(50).ToArray();

            return new Article(mediaId, externalArticle, keywords.Select(x => x.Value).ToArray());
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