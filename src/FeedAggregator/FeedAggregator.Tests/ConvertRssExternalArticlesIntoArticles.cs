using System;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Events;
using FluentAssertions;
using NSubstitute;
using Sociomedia.Domain;
using Sociomedia.Domain.Articles;
using Sociomedia.Domain.Medias;
using Sociomedia.FeedAggregator.Application.SynchronizeAllMediaFeeds;
using Xunit;

namespace FeedAggregator.Tests
{
    public class ConvertRssExternalArticlesIntoArticles : AcceptanceTests
    {
        private readonly IFeedReader _feedReader = Substitute.For<IFeedReader>();

        public ConvertRssExternalArticlesIntoArticles()
        {
            Container.Inject(_feedReader);

            _feedReader
                .ReadArticles(Arg.Any<string>())
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
        public async Task Do_not_read_media_feed_if_removed()
        {
            var mediaId = Guid.NewGuid();

            await EventStore.Save(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 },
                new MediaFeedRemoved(mediaId, "https://www.test.com/rss.xml") { Version = 3 },
            });

            EventStore.CommitEvents();

            _feedReader
                .ReadArticles("https://www.test.com/rss.xml")
                .Returns(x => new[] {
                    new ExternalArticle {
                        Url = new Uri("https://www.test.com/newpage.html"),
                        PublishDate = new DateTime(2020, 04, 01)
                    }
                });

            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            await _feedReader
                .Received(0)
                .ReadArticles("https://www.test.com/rss.xml");
        }

        [Fact]
        public async Task Read_all_feeds_of_a_media()
        {
            var mediaId = Guid.NewGuid();

            await EventStore.Save(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left),
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml"),
                new MediaFeedAdded(mediaId, "https://www.test.com/rss2.xml"),
            });

            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            await _feedReader
                .Received(1)
                .ReadArticles("https://www.test.com/rss.xml");
            await _feedReader
                .Received(1)
                .ReadArticles("https://www.test.com/rss2.xml");
        }

        [Fact]
        public async Task Create_new_article_when_new_external_article()
        {
            // Arrange
            var mediaId = Guid.NewGuid();

            await EventStore.Save(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 }
            });

            EventStore.CommitEvents();

            _feedReader
                .ReadArticles("https://www.test.com/rss.xml")
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

        [Fact]
        public async Task Ignore_articles_already_imported()
        {
            // Arrange
            var mediaId = Guid.NewGuid();

            await EventStore.Save(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 },
                new ArticleImported(Guid.NewGuid(), "test", "test", DateTimeOffset.Now, "https://test/article", "", "articleExternalId", new string[0], mediaId) { Version = 1},
            });

            EventStore.CommitEvents();

            _feedReader
                .ReadArticles("https://www.test.com/rss.xml")
                .Returns(x => new[] {
                    new ExternalArticle {
                        Id = "articleExternalId",
                        Title = "my title",
                        Summary = "summary",
                        ImageUrl = null,
                        Url = new Uri("https://www.test.com/newpage.html"),
                        PublishDate = new DateTime(2020, 04, 01)
                    }
                });

            // Act
            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            // Assert
            (await EventStore.GetNewEvents())
                .OfType<ArticleUpdated>()
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ArticleUpdated(
                        default,
                        "my title",
                        "summary",
                        new DateTime(2020, 04, 01),
                        "https://www.test.com/newpage.html",
                        null
                    )
                }, x => x.ExcludeDomainEventTechnicalFields());
        }
    }
}