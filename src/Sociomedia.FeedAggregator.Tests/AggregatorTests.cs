using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using FluentAssertions;
using NSubstitute;
using Sociomedia.Articles.Domain;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.FeedAggregator.Application;
using Sociomedia.FeedAggregator.Infrastructure;
using Sociomedia.Medias.Domain;
using Xunit;

namespace Sociomedia.FeedAggregator.Tests
{
    public class AggregatorTests
    {
        private readonly Aggregator _aggregator;
        private readonly IEventPositionRepository _eventPositionRepository = Substitute.For<IEventPositionRepository>();
        private readonly IEventStoreExtended _eventStore = Substitute.For<IEventStoreExtended>();

        public AggregatorTests()
        {
            var container = ContainerBuilder.Build(new Configuration());

            container.Inject(_eventPositionRepository);
            container.Inject(_eventStore);

            _aggregator = container.GetInstance<Aggregator>();
        }

        [Fact]
        public async Task Initialize_event_position_to_event_store_current_global_stream_position_when_first_time()
        {
            const long currentPosition = 1500000L;

            _eventPositionRepository.GetLastProcessedPosition().Returns((long?) null);
            _eventStore.GetCurrentGlobalStreamPosition().Returns(currentPosition);

            await _aggregator.StartAggregation(new CancellationToken(false));

            await _eventPositionRepository.Received(1).Save(currentPosition);
        }
    }

    public class FullAggregatorTests
    {
        private readonly Aggregator _aggregator;
        private readonly IEventPositionRepository _eventPositionRepository = Substitute.For<IEventPositionRepository>();
        private readonly IFeedReader _feedReader = Substitute.For<IFeedReader>();
        private readonly IKeywordDictionary _keywordDictionary = Substitute.For<IKeywordDictionary>();
        private readonly IWebPageDownloader _webPageDownloader = Substitute.For<IWebPageDownloader>();
        private readonly InMemoryEventStore _eventStore;
        private readonly InMemoryBus _inMemoryBus = new InMemoryBus();

        public FullAggregatorTests()
        {
            var container = ContainerBuilder.Build(new Configuration());

            _eventStore = container.GetInstance<InMemoryEventStore>();
            _keywordDictionary.IsValidKeyword(Arg.Any<string>()).Returns(true);
            _webPageDownloader.Download(Arg.Any<string>()).Returns("<html></html>");

            container.Inject(_feedReader);
            container.Inject(_eventPositionRepository);
            container.Inject(_keywordDictionary);
            container.Inject(_webPageDownloader);
            container.Inject((IEventStoreExtended) _eventStore);
            container.Inject((IEventStore) _eventStore);
            container.Inject((IEventBus) _inMemoryBus);
            container.Inject((ILogger) new EmptyLogger());

            _aggregator = container.GetInstance<Aggregator>();
        }

        [Fact]
        public async Task Simulate_full_process_with_one_article_already_synchronized_and_1_new()
        {
            // Arrange
            var source = new CancellationTokenSource(500);
            var mediaId = Guid.NewGuid();
            var articleId = Guid.NewGuid();

            await _eventStore.Store(new DomainEvent[] {
                new MediaFeedAdded(mediaId, "https://mysite/feed.xml") { Version = 1 },
                new ArticleImported(articleId, "some title", "some summary", DateTimeOffset.Now.Date, "https://mysite/article.html", null, "someExternalId", new string[0], mediaId) { Version = 1 }
            });
            _eventStore.CommitEvents();
            _eventPositionRepository.GetLastProcessedPosition().Returns(1);
            _feedReader.Read("https://mysite/feed.xml").Returns(new[] {
                new FeedItem { Id = "someExternalId", Title = "some title", Summary = "some summary", PublishDate = DateTimeOffset.Now.Date, Link = "https://mysite/article.html", ImageUrl = null },
                new FeedItem { Id = "someExternalId2", Title = "some title 2", Summary = "some summary 2", PublishDate = DateTimeOffset.Now.Date, Link = "https://mysite/article2.html", ImageUrl = "https://mysite/images/article2" },
            });

            // Act
            await _aggregator.StartAggregation(source.Token);
            await _inMemoryBus.Push(2, new ArticleImported(articleId, "some title", "some summary", DateTimeOffset.Now.Date, "https://mysite/article.html", null, "someExternalId", new string[0], mediaId) { Version = 1 });
            _inMemoryBus.SwitchToLiveMode();
            await Task.Delay(200, source.Token);
            foreach (var newEvent in await _eventStore.GetNewEvents()) {
                await _inMemoryBus.Push(3, (DomainEvent) newEvent);
            }

            // Asserts
            (await _eventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ArticleKeywordsDefined(default, new[] { new Keyword("some", 2) }),
                    new ArticleImported(default, "some title 2", "some summary 2", DateTimeOffset.Now.Date, "https://mysite/article2.html", "https://mysite/images/article2", "someExternalId2", new string[0], mediaId),
                    new ArticleKeywordsDefined(default, new[] { new Keyword("some", 2) }),
                }, x => x.ExcludeDomainEventTechnicalFields());

            await _eventPositionRepository.Received(1).Save(2);

            // Clean
            source.Cancel();
            source.Dispose();
        }
    }
}