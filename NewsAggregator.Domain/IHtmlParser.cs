namespace NewsAggregator.Domain {
    public interface IHtmlParser
    {
        string ExtractPlainTextArticleContent(string html);
    }
}