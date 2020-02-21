namespace NewsAggregator
{
    public class ArticleFactory
    {
        public Article Build(string html)
        {
            var articleContent = new HtmlParser().FindArticle(html);

            var plainText = new WordParser().ParseHtml(articleContent);

            return null;
        }
    }
}