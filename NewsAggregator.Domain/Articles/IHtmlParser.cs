namespace NewsAggregator.Domain.Articles
{
    public interface IHtmlParser
    {
        string ExtractPlainTextArticleContent(string html);
    }
}