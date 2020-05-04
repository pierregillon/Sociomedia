using System;
using System.Linq;
using System.Threading.Tasks;
using NewsAggregator.Domain.Rss;

namespace NewsAggregator.Domain.Articles
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

        public async Task<Article> Build(Guid rssSourceId, ExternalArticle externalArticle)
        {
            var html = await _htmlPageDownloader.Download(externalArticle.Url);

            var articleContent = _htmlParser.ExtractPlainTextArticleContent(html);

            var keywords = new KeywordsParser().Parse(articleContent).Take(50).ToArray();

            return new Article(externalArticle.Title, externalArticle.Url, rssSourceId, keywords);
        }
    }
}