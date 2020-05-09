using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CodeHollow.FeedReader.Feeds;
using HtmlAgilityPack;
using Sociomedia.Domain;
using Sociomedia.Domain.Articles;
using Sociomedia.FeedAggregator.Domain;

namespace Sociomedia.FeedAggregator.Infrastructure
{
    public class FeedParser : IFeedParser
    {
        private static readonly Regex RemoveDuplicatedSpacesRegex = new Regex(@"\s+", RegexOptions.Compiled);
        private readonly IHtmlParser htmlParser;

        public FeedParser(IHtmlParser htmlParser)
        {
            this.htmlParser = htmlParser;
        }

        public FeedContent Parse(Stream rssStream)
        {
            using var ms = new MemoryStream();
            rssStream.CopyTo(ms);
            var feed = CodeHollow.FeedReader.FeedReader.ReadFromByteArray(ms.ToArray());
            return new FeedContent(feed.Items.Select(ConvertToRssItem).ToArray());
        }

        private FeedItem ConvertToRssItem(CodeHollow.FeedReader.FeedItem syndicationItem)
        {
            return new FeedItem {
                Id = syndicationItem.Id,
                Title = WebUtility.HtmlDecode(syndicationItem.Title),
                Link = syndicationItem.Link,
                ImageUrl = GetImageUrl(syndicationItem)
                    .Pipe(UrlSanitizer.Sanitize),
                Summary = GetSummary(syndicationItem)
                    .Pipe(WebUtility.HtmlDecode)
                    .Pipe(HtmlToPlainText)
                    .Pipe(ClearConsecutiveSpaces),
                PublishDate = GetDate(syndicationItem)
            };
        }

        private static string GetSummary(CodeHollow.FeedReader.FeedItem syndicationItem)
        {
            if (!string.IsNullOrEmpty(syndicationItem.Description)) {
                return syndicationItem.Description;
            }
            return null;
        }

        private static DateTimeOffset GetDate(CodeHollow.FeedReader.FeedItem item)
        {
            if (item.SpecificItem is AtomFeedItem feedItem) {
                return ParseDate(feedItem.UpdatedDateString);
            }
            if (!string.IsNullOrEmpty(item.PublishingDateString)) {
                return ParseDate(item.PublishingDateString);
            }
            return default;
        }

        private string GetImageUrl(CodeHollow.FeedReader.FeedItem item)
        {
            if (item.SpecificItem is MediaRssFeedItem feedItem) {
                return feedItem.Media.FirstOrDefault()?.Url;
            }
            if (item.SpecificItem is AtomFeedItem atomFeedItem) {
                return atomFeedItem.Links.FirstOrDefault(x => x.LinkType?.StartsWith("image") == true)?.Href;
            }
            if (item.SpecificItem is Rss20FeedItem rss20FeedItem) {
                if (rss20FeedItem.Enclosure.MediaType?.StartsWith("image") == true) {
                    return rss20FeedItem.Enclosure.Url;
                }
                if (rss20FeedItem.Description?.Contains("<img") == true) {
                    return rss20FeedItem.Description
                        .Pipe(htmlParser.ExtractFirstImageUrl)
                        .Pipe(WebUtility.HtmlDecode);
                }
            }

            return null;
        }

        private static DateTimeOffset ParseDate(string dateStr)
        {
            if (DateTimeOffset.TryParse(dateStr, out var date)) {
                return date;
            }

            if (TryParseSpecialFrenchDateFormat(dateStr, out date)) {
                return date;
            }

            throw new InvalidOperationException($"The publish date '{dateStr}' has an invalid format.");
        }

        private static bool TryParseSpecialFrenchDateFormat(string dateStr, out DateTimeOffset date)
        {
            var match = Regex.Match(dateStr, @"\d{2}/\d{2}/\d{4}");
            if (match.Success) {
                dateStr = dateStr.Substring(dateStr.IndexOf(match.Value, StringComparison.InvariantCulture));
            }

            const string format = "MM/dd/yyyy - HH:mm";

            if (DateTimeOffset.TryParseExact(dateStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) {
                date = TimeZoneInfo.ConvertTime(
                    date.DateTime,
                    TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"),
                    TimeZoneInfo.Utc
                );
                return true;
            }

            return false;
        }

        private static string HtmlToPlainText(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) {
                return html;
            }
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument.DocumentNode.InnerText;
        }

        private static string ClearConsecutiveSpaces(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) {
                return text;
            }
            return RemoveDuplicatedSpacesRegex
                .Replace(text, " ")
                .Trim();
        }
    }
}