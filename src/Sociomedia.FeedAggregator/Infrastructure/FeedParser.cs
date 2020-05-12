using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using EventStore.ClientAPI;
using HtmlAgilityPack;
using Sociomedia.Domain;
using Sociomedia.Domain.Articles;
using Sociomedia.FeedAggregator.Domain;
using FeedItem = Sociomedia.FeedAggregator.Domain.FeedItem;

namespace Sociomedia.FeedAggregator.Infrastructure
{
    public class FeedParser : IFeedParser
    {
        private static readonly Regex RemoveDuplicatedSpacesRegex = new Regex(@"\s+", RegexOptions.Compiled);
        private readonly IHtmlParser _htmlParser;
        private readonly ILogger _logger;

        public FeedParser(IHtmlParser htmlParser, ILogger logger)
        {
            _htmlParser = htmlParser;
            _logger = logger;
        }

        public FeedContent Parse(Stream rssStream)
        {
            using var ms = new MemoryStream();
            rssStream.CopyTo(ms);

            return CodeHollow.FeedReader.FeedReader.ReadFromByteArray(ms.ToArray())
                .Pipe(GetFeedItems)
                .Pipe(x => x.ToArray())
                .Pipe(x => new FeedContent(x));
        }

        private IEnumerable<FeedItem> GetFeedItems(Feed feed)
        {
            foreach (var feedItem in feed.Items) {
                var rssItem = ConvertToRssItem(feedItem);
                if (rssItem.PublishDate != default) {
                    yield return rssItem;
                }
                else {
                    _logger.Info($"[FEED_PARSER] Unable to import feed {rssItem.Link} : publish date was not defined.");
                }
            }
        }

        private FeedItem ConvertToRssItem(CodeHollow.FeedReader.FeedItem syndicationItem)
        {
            return new FeedItem {
                Id = syndicationItem.Id,
                Title = WebUtility.HtmlDecode(syndicationItem.Title),
                Link = syndicationItem.Link,
                PublishDate = GetDate(syndicationItem),
                ImageUrl = syndicationItem
                    .Pipe(GetImageUrl)
                    .Pipe(UrlSanitizer.Sanitize),
                Summary = syndicationItem
                    .Pipe(GetSummary)
                    .Pipe(WebUtility.HtmlDecode)
                    .Pipe(HtmlToPlainText)
                    .Pipe(ClearConsecutiveSpaces)
            };
        }

        private static string GetSummary(CodeHollow.FeedReader.FeedItem syndicationItem)
        {
            if (!string.IsNullOrEmpty(syndicationItem.Description)) {
                return syndicationItem.Description;
            }
            return null;
        }

        private string GetImageUrl(CodeHollow.FeedReader.FeedItem item)
        {
            if (item.SpecificItem is MediaRssFeedItem feedItem) {
                if (feedItem.Enclosure.MediaType?.StartsWith("image") == true) {
                    return feedItem.Enclosure.Url;
                }
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
                        .Pipe(_htmlParser.ExtractFirstImageUrl)
                        .Pipe(WebUtility.HtmlDecode);
                }
            }

            return null;
        }

        private static DateTimeOffset GetDate(CodeHollow.FeedReader.FeedItem item)
        {
            if (item.SpecificItem is AtomFeedItem feedItem) {
                if (!string.IsNullOrWhiteSpace(feedItem.UpdatedDateString)) {
                    return ParseDate(feedItem.UpdatedDateString);
                }
            }
            if (!string.IsNullOrEmpty(item.PublishingDateString)) {
                return ParseDate(item.PublishingDateString);
            }
            if (item.SpecificItem is Rss20FeedItem rss20FeedItem) {
                if (!string.IsNullOrWhiteSpace(rss20FeedItem.DC.DateString)) {
                    return ParseDate(rss20FeedItem.DC.DateString);
                }
            }
            if (TryExtractDateFromUrl(item.Link, out var date)) {
                return date;
            }

            return default;
        }

        private static bool TryExtractDateFromUrl(string url, out DateTimeOffset date)
        {
            date = default;

            var regex = new Regex(@"(\d{2}-\d{2}-\d{4})");

            var match = regex.Match(url);
            if (match.Success) {
                var dateStr = match.Value;
                if (DateTime.TryParseExact(dateStr, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateFr)) {
                    date = dateFr.ToFrenchOffset();
                    return true;
                }
            }

            return false;
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
            date = default;

            var match = Regex.Match(dateStr, @"\d{2}/\d{2}/\d{4}");
            if (match.Success) {
                dateStr = dateStr.Substring(dateStr.IndexOf(match.Value, StringComparison.InvariantCulture));
            }

            const string format = "MM/dd/yyyy - HH:mm";

            if (DateTime.TryParseExact(dateStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateFr)) {
                date = dateFr.ToFrenchOffset();
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

    public static class DateTimeExtensions
    {
        public static DateTimeOffset ToFrenchOffset(this DateTime date)
        {
            return TimeZoneInfo.ConvertTime(
                date,
                TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"),
                TimeZoneInfo.Utc
            );
        }
    }
}