using System.Diagnostics.Tracing;

namespace NewsAggregator
{
    public class ArticleFactory
    {
        public Article Build(string html)
        {
            var articleContent = new HtmlParser().FindArticle(html);

            var words = new WordParser().ParseHtml(articleContent);

            var keywords = new KeywordsCalculator().Calculate(words, 30);

            return null;
        }
    }
}