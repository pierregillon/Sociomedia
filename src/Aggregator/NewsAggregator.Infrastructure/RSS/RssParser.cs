using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace NewsAggregator.Infrastructure.RSS
{
    public class RssParser : IRssParser
    {
        public RssContent Parse(Stream rssStream)
        {
            using var reader = XmlReader.Create(rssStream);

            var rss = XElement.Load(reader);

            var mediaAttribute = rss.Attribute(XNamespace.Xmlns + "media");
            var media = mediaAttribute != null ? XNamespace.Get(mediaAttribute.Value) : null;

            return new RssContent(rss
                .Descendants("item")
                .Select(x => new RssItem {
                    Id = (string) x.Element("guid"),
                    Title = WebUtility.HtmlDecode((string) x.Element("title")),
                    Summary = WebUtility.HtmlDecode((string) x.Element("description")),
                    PublishDate = ParseDate((string) x.Element("pubDate")),
                    Link = (string) x.Element("link"),
                    ImageUrl = media != null ? (string)x.Element(media + "content")?.Attribute("url") : null
                })
                .ToArray()
            );
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