using System.Linq;

namespace NewsAggregator
{
    public class ArticleFactory
    {
        public Article Build(string html)
        {
            var articleContent = new HtmlParser().ExtractPlainTextArticleContent(html);

            var keywords = new KeywordsParser().Parse(articleContent).Take(50).ToArray();

            return new Article(keywords);
        }
    }
}