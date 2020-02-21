namespace NewsAggregator
{
    public class ArticleFactory
    {
        public Article Build(string html)
        {
            var plainText = new WordParser().ParseHtml(html);

            return null;
        }
    }
}