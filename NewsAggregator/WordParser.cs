using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace NewsAggregator
{
    public class WordParser
    {
        private static readonly Regex RemoveHtmlTagsRegex = new Regex(@"<[^>]*>", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex RemoveExtractSpacesRegex = new Regex(@"\s+", RegexOptions.Compiled);

        public IReadOnlyCollection<string> ParseHtml(string html)
        {
            var text = RemoveHtmlTagsRegex.Replace(html, string.Empty);
            text = WebUtility.HtmlDecode(text);
            return RemoveExtractSpacesRegex.Replace(text, " ").Split(" ", StringSplitOptions.RemoveEmptyEntries);
        }
    }
}