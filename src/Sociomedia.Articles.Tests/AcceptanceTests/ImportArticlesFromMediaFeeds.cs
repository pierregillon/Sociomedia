using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Events;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sociomedia.Articles.Application.Commands.SynchronizeMediaFeeds;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Domain.Feeds;
using Sociomedia.Articles.Tests.UnitTests;
using Sociomedia.Core.Domain;
using Sociomedia.Medias.Domain;
using Xunit;

namespace Sociomedia.Articles.Tests.AcceptanceTests
{
    public class ImportArticlesFromMediaFeeds : AcceptanceTests
    {
        private readonly IFeedReader _feedReader = Substitute.For<IFeedReader>();

        public ImportArticlesFromMediaFeeds()
        {
            Container.Inject(_feedReader);

            _feedReader
                .Read(Arg.Any<string>())
                .Returns(x => Array.Empty<FeedItem>());

            WebPageDownloader
                .Download(Arg.Any<string>())
                .Returns("<html>bla</html>");

            WebPageDownloader
                .DownloadStream(Arg.Any<string>())
                .Returns(new MemoryStream(0));
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
        public async Task Do_not_create_any_articles_when_no_mandatory_fields_in_feed_item()
        {
            // Arrange
            var mediaId = Guid.NewGuid();

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 }
            });

            EventStore.CommitEvents();

            static FeedItem BuildNewFeedItem(string id = "someExternalId", string title = "some title", string link = "https://www.test.com/newpage.html", bool publishDateDefined = true) =>
                new FeedItem {
                    Id = id,
                    Title = title,
                    Summary = "some description",
                    Link = link,
                    PublishDate = publishDateDefined ? DateTimeOffset.Now : (DateTimeOffset?) null,
                    ImageUrl = "https://test/image.jpg"
                };

            _feedReader
                .Read("https://www.test.com/rss.xml")
                .Returns(x => new[] {
                    BuildNewFeedItem(""),
                    BuildNewFeedItem(title: ""),
                    BuildNewFeedItem(link: ""),
                    BuildNewFeedItem(publishDateDefined: false),
                });

            // Act
            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            // Assert
            (await EventStore.GetNewEvents())
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Do_not_create_any_articles_when_no_new_feed_items()
        {
            await EventStore.StoreAndPublish(new IEvent[] {
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

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 },
                new MediaFeedRemoved(mediaId, "https://www.test.com/rss.xml") { Version = 3 },
            });

            EventStore.CommitEvents();
            _feedReader.ClearReceivedCalls();

            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            await _feedReader
                .Received(0)
                .Read(Arg.Any<string>());
        }

        [Fact]
        public async Task Read_all_feeds_of_a_media()
        {
            var mediaId = Guid.NewGuid();

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left),
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml"),
                new MediaFeedAdded(mediaId, "https://www.test.com/rss2.xml"),
            });

            _feedReader.ClearReceivedCalls();

            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            await _feedReader.Received(2).Read(Arg.Any<string>());
            await _feedReader.Received(1).Read("https://www.test.com/rss.xml");
            await _feedReader.Received(1).Read("https://www.test.com/rss2.xml");
        }

        [Fact]
        public async Task Create_new_article_when_new_feed_content()
        {
            // Arrange
            var mediaId = Guid.NewGuid();

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 }
            });

            EventStore.CommitEvents();

            _feedReader
                .Read("https://www.test.com/rss.xml")
                .Returns(x => new[] {
                    new FeedItem {
                        Id = "someExternalId",
                        Title = "some title",
                        Summary = "some description",
                        Link = "https://www.test.com/newpage.html",
                        PublishDate = new DateTime(2020, 04, 01),
                        ImageUrl = "https://test/image.jpg"
                    }
                });

            // Act
            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            // Assert
            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ArticleImported(
                        default,
                        "some title",
                        "some description",
                        new DateTime(2020, 04, 01),
                        "https://www.test.com/newpage.html",
                        "https://test/image.jpg",
                        "someExternalId",
                        Array.Empty<string>(), mediaId
                    )
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Update_articles_if_feed_contains_newer_version()
        {
            // Arrange
            var mediaId = Guid.NewGuid();

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 },
                new ArticleImported(Guid.NewGuid(), "test", "test", new DateTime(2020, 04, 01), "https://test/article", null, "articleExternalId", new string[0], mediaId) { Version = 1 },
            });

            EventStore.CommitEvents();

            _feedReader
                .Read("https://www.test.com/rss.xml")
                .Returns(x => new[] {
                    new FeedItem {
                        Id = "articleExternalId",
                        Title = "my title",
                        Summary = "summary",
                        ImageUrl = "https://test/image.jpg",
                        Link = "https://www.test.com/newpage.html",
                        PublishDate = new DateTime(2020, 04, 02)
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
                        new DateTime(2020, 04, 02),
                        "https://www.test.com/newpage.html",
                        "https://test/image.jpg"
                    )
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Do_not_update_articles_if_feed_contains_current_version()
        {
            // Arrange
            var mediaId = Guid.NewGuid();

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 },
                new ArticleImported(Guid.NewGuid(), "test", "test", new DateTime(2020, 04, 01), "https://test/article", "", "articleExternalId", new string[0], mediaId) { Version = 1 },
            });

            EventStore.CommitEvents();

            _feedReader
                .Read("https://www.test.com/rss.xml")
                .Returns(x => new[] {
                    new FeedItem {
                        Id = "articleExternalId",
                        Title = "my title",
                        Summary = "summary",
                        ImageUrl = null,
                        Link = "https://www.test.com/newpage.html",
                        PublishDate = new DateTime(2020, 04, 01)
                    }
                });

            // Act
            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            // Assert
            (await EventStore.GetNewEvents())
                .OfType<ArticleUpdated>()
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Do_not_update_articles_if_feed_contains_already_updated_version()
        {
            // Arrange
            var mediaId = Guid.NewGuid();
            var articleId = Guid.NewGuid();

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 },
                new ArticleImported(articleId, "test", "test", new DateTime(2020, 04, 01), "https://test/article", "", "articleExternalId", new string[0], mediaId) { Version = 1 },
                new ArticleUpdated(articleId, "test", "test", new DateTime(2020, 04, 02), "https://test/article", "") { Version = 2 },
            });

            EventStore.CommitEvents();

            _feedReader
                .Read("https://www.test.com/rss.xml")
                .Returns(x => new[] {
                    new FeedItem {
                        Id = "articleExternalId",
                        Title = "my title",
                        Summary = "summary",
                        ImageUrl = null,
                        Link = "https://www.test.com/newpage.html",
                        PublishDate = new DateTime(2020, 04, 02)
                    }
                });

            // Act
            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            // Assert
            (await EventStore.GetNewEvents())
                .OfType<ArticleUpdated>()
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Do_not_import_feed_if_feed_url_unreachable()
        {
            WebPageDownloader
                .DownloadStream("https://www.test.com/rss.xml")
                .Throws(new UnreachableWebDocumentException());

            // Arrange
            var mediaId = Guid.NewGuid();

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 }
            });

            EventStore.CommitEvents();

            // Act
            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            // Assert
            (await EventStore.GetNewEvents())
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Ignore_empty_feed_url()
        {
            // Arrange
            var mediaId = Guid.NewGuid();

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "") { Version = 2 }
            });

            EventStore.CommitEvents();
            _feedReader.ClearReceivedCalls();

            // Act
            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            // Assert
            await _feedReader
                .Received(0)
                .Read("");

            (await EventStore.GetNewEvents())
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Do_not_update_articles_if_media_deleted()
        {
            // Arrange
            var mediaId = Guid.NewGuid();

            await EventStore.StoreAndPublish(new IEvent[] {
                new MediaAdded(mediaId, "test", null, PoliticalOrientation.Left) { Version = 1 },
                new MediaFeedAdded(mediaId, "https://www.test.com/rss.xml") { Version = 2 },
                new ArticleImported(Guid.NewGuid(), "test", "test", new DateTime(2020, 04, 01), "https://test/article", "", "articleExternalId", new string[0], mediaId) { Version = 1 },
                new MediaDeleted(mediaId) { Version = 2 },
            });

            EventStore.CommitEvents();
            _feedReader.ClearReceivedCalls();

            // Act
            await CommandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());

            // Assert
            await _feedReader
                .Received(0)
                .Read(Arg.Any<string>());
        }
    }
}