namespace Sociomedia.Articles.Domain
{
    public interface IHtmlParser
    {
        string ExtractPlainTextArticleContent(string html);
        string ExtractArticleImageUrl(string html);
        string ExtractFirstImageUrl(string html);
    }
}