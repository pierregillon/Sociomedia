using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Sociomedia.Domain;
using Sociomedia.Domain.Articles;

namespace Sociomedia.Infrastructure
{
    public class HtmlParser : IHtmlParser
    {
        private static readonly Regex RemoveDuplicatedSpacesRegex = new Regex(@"\s+", RegexOptions.Compiled);

        public string ExtractPlainTextArticleContent(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(html));

            return html
                .Pipe(WebUtility.HtmlDecode)
                .Pipe(FindPlainText)
                .Pipe(ClearConsecutiveSpaces)
                .Trim();
        }

        public string ExtractArticleImage(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var article = FindNode(htmlDocument.DocumentNode, "article");
            if (article == null) {
                return null;
            }
            var figure = FindNode(article, "figure");
            return FindFirstImgNode(figure ?? article);
        }

        private static string FindFirstImgNode(HtmlNode node)
        {
            var firstImgTag = FindNode(node, "img");
            if (firstImgTag == null) {
                return null;
            }
            return firstImgTag.Attributes["src"]?.Value ?? firstImgTag.Attributes["srcset"]?.Value.Split(' ').First();
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