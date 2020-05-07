using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sociomedia.Domain.Articles
{
    public class ArticleFactory
    {
        private readonly IHtmlParser _htmlParser;
        private readonly IHtmlPageDownloader _htmlPageDownloader;

        public ArticleFactory(IHtmlParser htmlParser, IHtmlPageDownloader htmlPageDownloader)
        {
            _htmlParser = htmlParser;
            _htmlPageDownloader = htmlPageDownloader;
        }

        public async Task<Article> Build(Guid mediaId, ExternalArticle externalArticle)
        {
            var html = await _htmlPageDownloader.Download(externalArticle.Url);
            if (string.IsNullOrEmpty(html)) {
                throw new ArticleNotFound($"The article with url '{externalArticle.Url}' was not found.");
            }

            var articleContent = _htmlParser.ExtractPlainTextArticleContent(html);

            var keywords = new KeywordsParser().Parse(articleContent).Take(50).ToArray();

            return new Article(mediaId, externalArticle, keywords.Select(x => x.ToString()).ToArray());
        }
    }

    public class ArticleNotFound : Exception
    {
        public ArticleNotFound(string message) : base(message) { }
    }
}