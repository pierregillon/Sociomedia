using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace NewsAggregator
{
    public class WordParser
    {
        private static readonly string[] Separators = { " ", "\"", "'", "«", "»", "?", "!", ";", ",", "." };

        private static readonly Regex RemoveDuplicatedSpacesRegex = new Regex(@"\s+", RegexOptions.Compiled);

        public IReadOnlyCollection<string> ParseHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(WebUtility.HtmlDecode(html));
            var text = doc.DocumentNode.InnerText;
            return RemoveDuplicatedSpacesRegex
                .Replace(text, " ")
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}