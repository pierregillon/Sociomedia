using System;
using System.Linq;
using System.Threading.Tasks;
using Sociomedia.FeedAggregator.Domain.Medias;

namespace Sociomedia.FeedAggregator.Domain.Articles
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

            var articleContent = _htmlParser.ExtractPlainTextArticleContent(html);

            var keywords = new KeywordsParser().Parse(articleContent).Take(50).ToArray();

            return new Article(mediaId, externalArticle, keywords.Select(x => x.ToString()).ToArray());
        }
    }
}