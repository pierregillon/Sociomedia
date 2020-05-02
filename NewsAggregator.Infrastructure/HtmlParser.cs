using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NewsAggregator.Domain.Articles;

namespace NewsAggregator.Infrastructure
{
    public class HtmlParser : IHtmlParser
    {
        private static readonly Regex RemoveDuplicatedSpacesRegex = new Regex(@"\s+", RegexOptions.Compiled);

        public string ExtractPlainTextArticleContent(string html)
        {
            var text = FindPlainText(WebUtility.HtmlDecode(html));
            return RemoveDuplicatedSpacesRegex.Replace(text, " ").Trim();
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

            foreach (var childNode in node.ChildNodes) {
                var result = FindNode(childNode, name);
                if (result != null) return result;
            }

            return null;
        }
    }
}