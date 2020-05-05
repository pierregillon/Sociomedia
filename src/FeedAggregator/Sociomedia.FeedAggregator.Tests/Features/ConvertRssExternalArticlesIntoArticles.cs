using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sociomedia.FeedAggregator.Application.Commands.SynchronizeRssSources;
using Sociomedia.FeedAggregator.Domain;
using Sociomedia.FeedAggregator.Domain.Rss;
using Xunit;

namespace Sociomedia.FeedAggregator.Tests.Features
{
    public class ConvertRssExternalArticlesIntoArticles : AcceptanceTests
    {
        private readonly IRssSourceReader _rssSourceReader = Substitute.For<IRssSourceReader>();

        public ConvertRssExternalArticlesIntoArticles()
        {
            Container.Inject(_rssSourceReader);

            _rssSourceReader
                .ReadNewArticles(Arg.Any<Uri>(), Arg.Any<DateTimeOffset?>())
                .Returns(x => Array.Empty<ExternalArticle>());
        }

        [Fact]
        public async Task Do_not_create_any_articles_when_no_sources()
        {
            await CommandDispatcher.Dispatch(new SynchronizeRssSourcesCommand());

            (await EventStore.GetAllEvents())
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Do_not_create_any_articles_when_no_new_rss_external_articles()
        {
            await EventStore.Save(new IDomainEvent[] {
                new RssSourceAdded(Guid.Empty, new Uri("https://www.test.com/rss.xml"))
            });

            await CommandDispatcher.Dispatch(new SynchronizeRssSourcesCommand());

            (await EventStore.GetAllEvents())
                .Skip(1)
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Get_all_external_articles_when_source_never_synchronized()
        {
            var sourceId = Guid.NewGuid();

            await EventStore.Save(new IDomainEvent[] {
                new RssSourceAdded(sourceId, new Uri("https://www.test.com/rss.xml")) { Version = 1 }
            });

            await CommandDispatcher.Dispatch(new SynchronizeRssSourcesCommand());

            await _rssSourceReader
                .Received(1)
                .ReadNewArticles(new Uri("https://www.test.com/rss.xml"), null);
        }

        [Fact]
        public async Task Get_new_external_articles_from_last_synchronization_date_when_source_has_been_synchronized()
        {
            var sourceId = Guid.NewGuid();

            await EventStore.Save(new IDomainEvent[] {
                new RssSourceAdded(sourceId, new Uri("https://www.test.com/rss.xml")) { Version = 1 },
                new RssSourceSynchronized(sourceId, new DateTime(2020, 05, 01)) { Version = 2 }
            });

            await CommandDispatcher.Dispatch(new SynchronizeRssSourcesCommand());

            await _rssSourceReader
                .Received(1)
                .ReadNewArticles(new Uri("https://www.test.com/rss.xml"), new DateTime(2020, 05, 01));
        }

        [Fact]
        public async Task Create_new_article_when_new_external_article()
        {
            var sourceId = Guid.NewGuid();

            await EventStore.Save(new IDomainEvent[] {
                new RssSourceAdded(sourceId, new Uri("https://www.test.com/rss.xml")) { Version = 1 },
                new RssSourceSynchronized(sourceId, new DateTime(2020, 05, 01)) { Version = 2 }
            });

            _rssSourceReader
                .ReadNewArticles(new Uri("https://www.test.com/rss.xml"), new DateTime(2020, 05, 01))
                .Returns(x => new[] {
                    new ExternalArticle {
                        Url = new Uri("https://www.test.com/newpage.html"),
                        PublishDate = new DateTime(2020, 04, 01)
                    }
                });

            await CommandDispatcher.Dispatch(new SynchronizeRssSourcesCommand());

            (await EventStore.GetAllEvents())
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