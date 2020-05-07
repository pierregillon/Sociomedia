using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CodeHollow.FeedReader.Feeds;
using Sociomedia.FeedAggregator.Domain;
using FeedItem = Sociomedia.FeedAggregator.Domain.FeedItem;

namespace Sociomedia.FeedAggregator.Infrastructure
{
    public class FeedParser : IFeedParser
    {
        public FeedContent Parse(Stream rssStream)
        {
            using var ms = new MemoryStream();
            rssStream.CopyTo(ms);
            var feed = CodeHollow.FeedReader.FeedReader.ReadFromByteArray(ms.ToArray());
            return new FeedContent(feed.Items.Select(ConvertToRssItem).ToArray());
        }

        private static FeedItem ConvertToRssItem(CodeHollow.FeedReader.FeedItem syndicationItem)
        {
            return new FeedItem {
                Id = syndicationItem.Id,
                Title = WebUtility.HtmlDecode(syndicationItem.Title),
                Link = syndicationItem.Link,
                ImageUrl = GetImageUrl(syndicationItem),
                Summary = WebUtility.HtmlDecode(GetSummary(syndicationItem)),
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
            if (!string.IsNullOrEmpty(item.PublishingDateString)) {
                return ParseDate(item.PublishingDateString);
            }
            if (item.SpecificItem is AtomFeedItem feedItem) {
                return ParseDate(feedItem.UpdatedDateString);
            }
            return default;
        }

        private static string GetImageUrl(CodeHollow.FeedReader.FeedItem item)
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
    }
}