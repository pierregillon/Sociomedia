using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;

namespace NewsAggregator.Infrastructure.RSS
{
    public class RssParser : IRssParser
    {
        public RssContent Parse(Stream rssStream)
        {
            using var ms = new MemoryStream();
            rssStream.CopyTo(ms);
            var feed = FeedReader.ReadFromByteArray(ms.ToArray());
            return new RssContent(feed.Items.Select(ConvertToRssItem).ToArray());
        }

        private static RssItem ConvertToRssItem(FeedItem syndicationItem)
        {
            return new RssItem {
                Id = syndicationItem.Id,
                Title = WebUtility.HtmlDecode(syndicationItem.Title),
                Link = syndicationItem.Link,
                ImageUrl = GetImageUrl(syndicationItem),
                Summary = WebUtility.HtmlDecode(GetSummary(syndicationItem)),
                PublishDate = GetDate(syndicationItem)
            };
        }

        private static string GetSummary(FeedItem syndicationItem)
        {
            if (!string.IsNullOrEmpty(syndicationItem.Description)) {
                return syndicationItem.Description;
            }
            return null;
        }

        private static DateTimeOffset GetDate(FeedItem item)
        {
            if (!string.IsNullOrEmpty(item.PublishingDateString)) {
                return ParseDate(item.PublishingDateString);
            }
            if (item.SpecificItem is AtomFeedItem feedItem) {
                return ParseDate(feedItem.UpdatedDateString);
            }
            return default;
        }

        private static string GetImageUrl(FeedItem item)
        {
            if (item.SpecificItem is MediaRssFeedItem feedItem) {
                return feedItem.Media.FirstOrDefault()?.Url;
            }
            if (item.SpecificItem is AtomFeedItem atomFeedItem) {
                return atomFeedItem.Links.FirstOrDefault(x => x.LinkType == "image/jpeg")?.Href;
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

            if (DateTimeOffset.TryParseExact(dateStr, format, new CultureInfo("fr-FR"), DateTimeStyles.AssumeLocal, out date)) {
                return true;
            }

            return false;
        }
    }
}