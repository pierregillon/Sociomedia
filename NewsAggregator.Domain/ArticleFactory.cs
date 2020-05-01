using System.Linq;

namespace NewsAggregator.Domain
{
    public class ArticleFactory
    {
        private readonly IHtmlParser _htmlParser;

        public ArticleFactory(IHtmlParser htmlParser)
        {
            _htmlParser = htmlParser;
        }

        public Article Build(string url, string html)
        {
            var articleContent = _htmlParser.ExtractPlainTextArticleContent(html);

            var keywords = new KeywordsParser().Parse(articleContent).Take(50).ToArray();

            return new Article(url, keywords);
        }
    }
}