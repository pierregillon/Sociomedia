using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Events;
using FluentAssertions;
using NSubstitute;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Tests.UnitTests;
using Sociomedia.Core.Domain;
using Sociomedia.Medias.Domain;
using Xunit;

namespace Sociomedia.Articles.Tests.AcceptanceTests
{
    public class SynchronizeFeedOnMediaFeedAdded : AcceptanceTests
    {
        private readonly IFeedReader _feedReader = Substitute.For<IFeedReader>();

        public SynchronizeFeedOnMediaFeedAdded()
        {
            Container.Inject(_feedReader);
        }

        [Fact]
        public async Task Synchronize_feed_on_media_feed_added()
        {
            var mediaId = Guid.NewGuid();

            await EventStore.Save(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left)
            });

            EventStore.CommitEvents();

            _feedReader
                .Read("https://www.test.com/rss.xml")
                .Returns(x => new[] {
                    new FeedItem {
                        Id = "feedItem1",
                        Title = "coronavirus increases",
                        Summary = "some summary",
                        Link = "https://news/article1.html",
                        PublishDate = DateTimeOffset.Now.Date,
                        ImageUrl = "https://news/article1.jpg"
                    },
                    new FeedItem {
                        Id = "feedItem2",
                        Title = "get some masks",
                        Summary = "other summary",
                        Link = "https://news/article2.html",
                        PublishDate = DateTimeOffset.Now.Date,
                        ImageUrl = "https://news/article2.jpg"
                    }
                });

            await EventStore.Save(new IEvent[] {
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml"),
            });

            (await EventStore.GetNewEvents())
                .OfType<ArticleImported>()
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ArticleImported(default, "coronavirus increases", "some summary", DateTimeOffset.Now.Date, "https://news/article1.html", "https://news/article1.jpg", "feedItem1", new string[0], mediaId),
                    new ArticleImported(default, "get some masks", "other summary", DateTimeOffset.Now.Date, "https://news/article2.html", "https://news/article2.jpg", "feedItem2", new string[0], mediaId),
                }, x => x.ExcludeDomainEventTechnicalFields());
        }
    }
}