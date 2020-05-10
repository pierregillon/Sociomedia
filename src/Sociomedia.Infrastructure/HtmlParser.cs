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
        private static readonly string[] _figureClasses = { "article-full__cover", "article-full__content", "c-article-media__img" };
        private static readonly string[] _figureTags = { "figure", "picture" };

        public string ExtractPlainTextArticleContent(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(html));

            return html
                .Pipe(WebUtility.HtmlDecode)
                .Pipe(FindPlainText)
                .Pipe(ClearConsecutiveSpaces)
                .Trim();
        }

        public string ExtractArticleImageUrl(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var article = FindNode(htmlDocument.DocumentNode, "article");
            if (article == null) {
                return null;
            }
            var figure = FindFigureNode(article);
            if (figure != null) {
                return FindFirstImgNode(figure);
            }
            return null;
        }

        public string ExtractFirstImageUrl(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return FindFirstImgNode(htmlDocument.DocumentNode);
        }

        private static string FindFirstImgNode(HtmlNode node)
        {
            var firstImgTag = FindNode(node, "img");
            if (firstImgTag == null) {
                return null;
            }
            return firstImgTag.Attributes["data-src"]?.Value
                   ?? firstImgTag.Attributes["src"]?.Value
                   ?? firstImgTag.Attributes["srcset"]?.Value.Split(' ').First();
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

        private static HtmlNode FindFigureNode(HtmlNode node)
        {
            return FindFigureNodeByTag(node) ?? FindFigureNodeByClassName(node);
        }

        private static HtmlNode FindFigureNodeByTag(HtmlNode node)
        {
            foreach (var figureTag in _figureTags) {
                var figure = FindNode(node, figureTag);
                if (figure != null) {
                    return figure;
                }
            }
            return null;
        }

        private static HtmlNode FindFigureNodeByClassName(HtmlNode node)
        {
            foreach (var figureClass in _figureClasses) {
                var figure = FindNodeByClassName(node, figureClass);
                if (figure != null) {
                    return figure;
                }
            }
            return null;
        }

        private static HtmlNode FindNode(HtmlNode node, string name)
        {
            if (node.Name == name) return node;

            return node.ChildNodes
                .Select(childNode => FindNode(childNode, name))
                .FirstOrDefault(result => result != null);
        }

        private static HtmlNode FindNodeByClassName(HtmlNode node, string className)
        {
            if (node.Attributes.Contains("class") && node.Attributes["class"].Value.Split(' ').Contains(className))
                return node;

            return node.ChildNodes
                .Select(childNode => FindNodeByClassName(childNode, className))
                .FirstOrDefault(result => result != null);
        }
    }
}