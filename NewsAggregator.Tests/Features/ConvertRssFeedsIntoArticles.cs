using System;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Events;
using FluentAssertions;
using NewsAggregator.Application.Commands.SynchronizeRssFeed;
using NewsAggregator.Domain;
using NewsAggregator.Domain.Articles;
using NewsAggregator.Domain.Rss;
using NewsAggregator.Infrastructure;
using NewsAggregator.Infrastructure.CQRS;
using NSubstitute;
using Xunit;

namespace NewsAggregator.Tests.Features
{
    public class ConvertRssFeedsIntoArticles
    {
        private readonly IRssFeedReader _rssFeedReader = Substitute.For<IRssFeedReader>();
        private readonly IHtmlPageDownloader _htmlPageDownloader = Substitute.For<IHtmlPageDownloader>();
        private readonly InMemoryEventStore _eventStore;
        private readonly ICommandDispatcher _commandDispatcher;

        public ConvertRssFeedsIntoArticles()
        {
            var container = ContainerBuilder.Build();
            container.Inject(_rssFeedReader);
            container.Inject(_htmlPageDownloader);

            _commandDispatcher = container.GetInstance<ICommandDispatcher>();
            _eventStore = (InMemoryEventStore) container.GetInstance<IEventStore>();

            _rssFeedReader
                .ReadNewFeeds(Arg.Any<Uri>(), Arg.Any<DateTime?>())
                .Returns(x => new RssFeeds(Array.Empty<RssFeed>()));
        }

        [Fact]
        public async Task Do_not_create_any_articles_when_no_sources()
        {
            await _commandDispatcher.Dispatch(new SynchronizeRssFeedCommand());

            (await _eventStore.GetAllEvents())
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Do_not_create_any_articles_when_no_new_rss_feeds()
        {
            await _eventStore.Save(new IDomainEvent[] {
                new RssSourceAdded(Guid.Empty, new Uri("https://www.test.com/rss.xml"))
            });

            await _commandDispatcher.Dispatch(new SynchronizeRssFeedCommand());

            (await _eventStore.GetAllEvents())
                .Skip(1)
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Get_all_feeds_when_source_never_synchronized()
        {
            var sourceId = Guid.NewGuid();

            await _eventStore.Save(new IDomainEvent[] {
                new RssSourceAdded(sourceId, new Uri("https://www.test.com/rss.xml")) { Version = 1 }
            });

            await _commandDispatcher.Dispatch(new SynchronizeRssFeedCommand());

            await _rssFeedReader
                .Received(1)
                .ReadNewFeeds(new Uri("https://www.test.com/rss.xml"), null);
        }

        [Fact]
        public async Task Get_new_feeds_from_last_synchronization_date_when_source_has_been_synchronized()
        {
            var sourceId = Guid.NewGuid();

            await _eventStore.Save(new IDomainEvent[] {
                new RssSourceAdded(sourceId, new Uri("https://www.test.com/rss.xml")) { Version = 1 },
                new RssSourceSynchronized(sourceId, new DateTime(2020, 05, 01)) { Version = 2 }
            });

            await _commandDispatcher.Dispatch(new SynchronizeRssFeedCommand());

            await _rssFeedReader
                .Received(1)
                .ReadNewFeeds(new Uri("https://www.test.com/rss.xml"), new DateTime(2020, 05, 01));
        }

        [Fact]
        public async Task Create_new_article_when_new_feed()
        {
            var sourceId = Guid.NewGuid();

            await _eventStore.Save(new IDomainEvent[] {
                new RssSourceAdded(sourceId, new Uri("https://www.test.com/rss.xml")) { Version = 1 },
                new RssSourceSynchronized(sourceId, new DateTime(2020, 05, 01)) { Version = 2 }
            });

            _rssFeedReader
                .ReadNewFeeds(new Uri("https://www.test.com/rss.xml"), new DateTime(2020, 05, 01))
                .Returns(x => new RssFeeds(new[] {
                    new RssFeed {
                        Url = new Uri("https://www.test.com/newpage.html"),
                        PublishDate = new DateTime(2020, 04, 01)
                    }
                }));

            await _commandDispatcher.Dispatch(new SynchronizeRssFeedCommand());

            (await _eventStore.GetAllEvents())
                .Skip(2)
                .Should()
                .ContainEquivalentOf(new {
                    Url = new Uri("https://www.test.com/newpage.html"),
                    RssSourceId = sourceId,
                    Keywords = new Keyword[0],
                    Version = 1
                });
        }
    }
}