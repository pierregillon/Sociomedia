using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<Article> Build(Uri url, Guid rssSourceId)
        {
            var html = await _htmlPageDownloader.Download(url);

            var articleContent = _htmlParser.ExtractPlainTextArticleContent(html);

            var keywords = new KeywordsParser().Parse(articleContent).Take(50).ToArray();

            return new Article("test", url, rssSourceId, keywords);
        }
    }
}