using System;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Events;
using FluentAssertions;
using NSubstitute;
using Sociomedia.DomainEvents.Media;
using Sociomedia.FeedAggregator.Application.Commands.SynchronizeAllMediaFeeds;
using Sociomedia.FeedAggregator.Domain.Articles;
using Sociomedia.FeedAggregator.Domain.Medias;
using Xunit;
using MediaAdded = Sociomedia.FeedAggregator.Domain.Medias.MediaAdded;
using MediaFeedAdded = Sociomedia.FeedAggregator.Domain.Medias.MediaFeedAdded;
using MediaFeedSynchronized = Sociomedia.FeedAggregator.Domain.Medias.MediaFeedSynchronized;

namespace Sociomedia.FeedAggregator.Tests.Features
{
    public class ConvertRssExternalArticlesIntoArticles : AcceptanceTests
    {
        private readonly IFeedReader _feedReader = Substitute.For<IFeedReader>();

        public ConvertRssExternalArticlesIntoArticles()
        {
            Container.Inject(_feedReader);

            _feedReader
                .ReadNewArticles(Arg.Any<string>(), Arg.Any<DateTimeOffset?>())
                .Returns(x => Array.Empty<ExternalArticle>());
        }

        [Fact]
        public async Task Do_not_create_any_articles_when_no_sources()
        {
            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            (await EventStore.GetNewEvents())
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Do_not_create_any_articles_when_no_new_rss_external_articles()
        {
            await EventStore.Save(new IEvent[] {
                new MediaAdded(Guid.Empty, "test", null, PoliticalOrientation.Left),
                new MediaFeedAdded(Guid.Empty, "https://www.test.com/rss.xml")
            });

            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            (await EventStore.GetNewEvents())
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Get_all_external_articles_when_source_never_synchronized()
        {
            var mediaId = Guid.NewGuid();

            await EventStore.Save(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left),
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml")
            });

            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            await _feedReader
                .Received(1)
                .ReadNewArticles("https://www.test.com/rss.xml", null);
        }

        [Fact]
        public async Task Get_new_external_articles_from_last_synchronization_date_when_source_has_been_synchronized()
        {
            var mediaId = Guid.NewGuid();

            await EventStore.Save(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left),
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml"),
                new MediaFeedSynchronized(mediaId, "https://www.test.com/rss.xml", new DateTime(2020, 05, 01))
            });

            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            await _feedReader
                .Received(1)
                .ReadNewArticles("https://www.test.com/rss.xml", new DateTime(2020, 05, 01));
        }

        [Fact]
        public async Task Create_new_article_when_new_external_article()
        {
            // Arrange
            var mediaId = Guid.NewGuid();

            await EventStore.Save(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 },
                new MediaFeedSynchronized(mediaId, "https://www.test.com/rss.xml", new DateTime(2020, 05, 01)) { Version = 3 }
            });

            EventStore.CommitEvents();

            _feedReader
                .ReadNewArticles("https://www.test.com/rss.xml", new DateTime(2020, 05, 01))
                .Returns(x => new[] {
                    new ExternalArticle {
                        Url = new Uri("https://www.test.com/newpage.html"),
                        PublishDate = new DateTime(2020, 04, 01)
                    }
                });

            // Act
            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            // Assert
            var events = (await EventStore.GetNewEvents()).ToArray();

            events
                .OfType<ArticleImported>()
                .Should()
                .BeEquivalentTo(new {
                    Url = "https://www.test.com/newpage.html",
                    Keywords = Array.Empty<string>(),
                    Version = 1,
                    MediaId = mediaId
                });

            var mediaFeedSynchronized = events.OfType<MediaFeedSynchronized>().Single();
            mediaFeedSynchronized.Id.Should().Be(mediaId);
            mediaFeedSynchronized.SynchronizationDate.Date.Should().Be(DateTime.UtcNow.Date);
        }
    }
}