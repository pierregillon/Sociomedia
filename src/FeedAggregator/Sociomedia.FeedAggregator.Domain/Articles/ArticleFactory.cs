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

            externalArticle.ImageUrl ??= html.Pipe(_htmlParser.ExtractArticleImageUrl);

            var keywords = new KeywordsParser().Parse(articleContent).Take(50).ToArray();

            return new Article(mediaId, externalArticle, keywords.Select(x => x.ToString()).ToArray());
        }
    }
}