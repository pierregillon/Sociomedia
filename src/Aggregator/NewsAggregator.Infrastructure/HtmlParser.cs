using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NewsAggregator.Domain;
using NewsAggregator.Domain.Articles;

namespace NewsAggregator.Infrastructure
{
    public class HtmlParser : IHtmlParser
    {
        private static readonly Regex RemoveDuplicatedSpacesRegex = new Regex(@"\s+", RegexOptions.Compiled);

        public string ExtractPlainTextArticleContent(string html)
        {
            return html
                .Pipe(WebUtility.HtmlDecode)
                .Pipe(FindPlainText)
                .Pipe(ClearConsecutiveSpaces)
                .Trim();
        }

        private static string ClearConsecutiveSpaces(string text)
        {
            return RemoveDuplicatedSpacesRegex.Replace(text, " ");
        }

        private static string FindPlainText(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return FindNode(htmlDocument.DocumentNode, "article")?.InnerText
                   ?? htmlDocument.DocumentNode.InnerText;
        }

        private static HtmlNode FindNode(HtmlNode node, string name)
        {
            if (node.Name == name) return node;

            return node.ChildNodes
                .Select(childNode => FindNode(childNode, name))
                .FirstOrDefault(result => result != null);
        }
    }
}