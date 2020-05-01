using System;
using System.Linq;

namespace NewsAggregator.Domain.Articles
{
    public class ArticleFactory
    {
        private readonly IHtmlParser _htmlParser;

        public ArticleFactory(IHtmlParser htmlParser)
        {
            _htmlParser = htmlParser;
        }

        public Article Build(string url, string html, Guid rssSourceId)
        {
            var articleContent = _htmlParser.ExtractPlainTextArticleContent(html);

            var keywords = new KeywordsParser().Parse(articleContent).Take(50).ToArray();

            return new Article("test", url, rssSourceId, keywords);
        }
    }
}