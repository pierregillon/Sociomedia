namespace NewsAggregator
{
    public class ArticleFactory
    {
        public Article Build(string html)
        {
            var plainText = new HtmlParser().ParseToPlainText(html);

            return null;
        }
    }
}