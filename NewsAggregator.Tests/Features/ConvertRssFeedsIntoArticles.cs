using System;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Events;
using FluentAssertions;
using NewsAggregator.Application.Commands.SynchronizeRssFeed;
using NewsAggregator.Domain;
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
        private readonly IEventStore _eventStore;
        private readonly ICommandDispatcher _commandDispatcher;

        public ConvertRssFeedsIntoArticles()
        {
            var container = ContainerBuilder.Build();
            container.Inject(_rssFeedReader);

            _commandDispatcher = container.GetInstance<ICommandDispatcher>();
            _eventStore = container.GetInstance<IEventStore>();
        }

        [Fact]
        public async Task Do_not_create_any_articles_when_no_new_rss_feeds()
        {
            await _eventStore.Save(new IDomainEvent[] {
                new RssSourceAdded(Guid.Empty, "https://www.test.com/rss.xml"),
                new RssSourceSynchronized(Guid.Empty, new DateTime(2020, 05, 01))
            });

            _rssFeedReader
                .Read("https://www.test.com/rss.xml")
                .Returns(x => new RssFeeds(new[] {
                    new RssFeed { Id = "Test1", Url = "https://www.test.com/rss.xml", PublishDate = new DateTime(2020, 01, 01) }
                }));

            await _commandDispatcher.Dispatch(new SynchronizeRssFeedCommand());

            (await ((InMemoryEventStore) _eventStore).GetAllEvents())
                .Skip(2)
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Create_new_articles_when_new_feed()
        {
            var sourceId = Guid.NewGuid();
            await _eventStore.Save(new IDomainEvent[] {
                new RssSourceAdded(sourceId, "https://www.test.com/rss.xml") { Version = 1 },
                new RssSourceSynchronized(sourceId, new DateTime(2020, 05, 01)) { Version = 2 }
            });

            _rssFeedReader
                .Read("https://www.test.com/rss.xml")
                .Returns(x => new RssFeeds(new[] {
                    new RssFeed {
                        Id = "Test1", 
                        Url = "https://www.test.com/newpage.html", 
                        PublishDate = new DateTime(2020, 05, 02), 
                        Html = ""
                    }
                }));

            await _commandDispatcher.Dispatch(new SynchronizeRssFeedCommand());

            (await ((InMemoryEventStore) _eventStore).GetAllEvents())
                .Should()
                .ContainEquivalentOf(new {
                    Url = "https://www.test.com/newpage.html",
                    RssSourceId = sourceId,
                    Keywords = new Keyword[0],
                    Version = 1
                });
        }
    }
}