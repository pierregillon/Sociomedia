using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using FluentAssertions;
using Lamar;
using NSubstitute;
using Sociomedia.DomainEvents.Article;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ReadModel.DataAccess.Tables;
using Xunit;

namespace Sociomedia.ProjectionSynchronizer.Tests
{
    public class DomainEventSynchronizerTests
    {
        private readonly INestedContainer _container;
        private readonly InMemoryBus _inMemoryBus = new InMemoryBus();
        private readonly IArticleRepository _articleRepository = Substitute.For<IArticleRepository>();
        private readonly IStreamPositionRepository _streamPositionRepository = Substitute.For<IStreamPositionRepository>();

        public DomainEventSynchronizerTests()
        {
            var mainContainer = ContainerBuilder.Build(new Configuration());

            _container = mainContainer.GetNestedContainer();
            _container.Inject<IEventBus>(_inMemoryBus);
            _container.Inject<ILogger>(new EmptyLogger());
            _container.Inject(_articleRepository);
            _container.Inject(_streamPositionRepository);

            var configuration = _container.GetInstance<ProjectionSynchronizationConfiguration>();
            configuration.ReconnectionDelayMs = 1;
        }

        [Theory]
        [InlineData(null)]
        [InlineData(10)]
        public async Task Start_synchronization_from_last_stream_position(long? lastStreamPosition)
        {
            // Arrange
            var synchronizer = _container.GetInstance<DomainEventSynchronizer>();
            _streamPositionRepository.GetLastPosition().Returns(lastStreamPosition);

            // Act
            await synchronizer.StartSynchronization();

            // Asserts
            _inMemoryBus.LastStreamPosition
                .Should()
                .Be(lastStreamPosition);
        }

        [Fact]
        public async Task Create_article_when_receiving_article_synchronized_event()
        {
            var synchronizer = _container.GetInstance<DomainEventSynchronizer>();

            await synchronizer.StartSynchronization();

            // Acts

            var articleSynchronized = new ArticleSynchronized(
                Guid.NewGuid(),
                "My title",
                "This is a simple summary",
                new DateTimeOffset(2020, 05, 06, 10, 0, 0, TimeSpan.FromHours(2)),
                "https://test.com",
                "https://test/image/jpg",
                Array.Empty<string>(),
                Guid.NewGuid()
            );

            await _inMemoryBus.Push(1, articleSynchronized);

            // Asserts

            await _articleRepository.Received(1).AddArticle(Arg.Any<ArticleTable>());
            await _articleRepository
                .Received(1)
                .AddArticle(Arg.Is<ArticleTable>(article =>
                    article.Id == articleSynchronized.Id &&
                    article.Title == articleSynchronized.Title &&
                    article.ImageUrl == articleSynchronized.ImageUrl &&
                    article.Summary == articleSynchronized.Summary &&
                    article.PublishDate == articleSynchronized.PublishDate &&
                    article.Url == articleSynchronized.Url
                ));
        }

        [Fact]
        public async Task Create_keywords_when_receiving_article_synchronized_event()
        {
            var synchronizer = _container.GetInstance<DomainEventSynchronizer>();

            await synchronizer.StartSynchronization();

            // Acts

            var articleSynchronized = new ArticleSynchronized(
                Guid.NewGuid(),
                default,
                default,
                default,
                default,
                default,
                new[] { "coronavirus", "france", "pandemic" },
                Guid.NewGuid()
            );

            await _inMemoryBus.Push(1, articleSynchronized);

            // Asserts

            await _articleRepository.Received(3).AddKeywords(Arg.Any<KeywordTable>());

            foreach (var keyword in articleSynchronized.Keywords) {
                await _articleRepository
                    .Received(1)
                    .AddKeywords(Arg.Is<KeywordTable>(x =>
                        x.Value == keyword &&
                        x.FK_Article == articleSynchronized.Id
                    ));
            }
        }

        [Fact]
        public async Task Update_last_position_in_stream_for_each_events()
        {
            var synchronizer = _container.GetInstance<DomainEventSynchronizer>();

            await synchronizer.StartSynchronization();

            await _inMemoryBus.Push(1, SomeArticleSynchronized());
            await _inMemoryBus.Push(2, SomeArticleSynchronized());
            await _inMemoryBus.Push(3, SomeArticleSynchronized());

            await _streamPositionRepository.Received(3).Save(Arg.Any<long>());
            await _streamPositionRepository.Received(1).Save(1);
            await _streamPositionRepository.Received(1).Save(2);
            await _streamPositionRepository.Received(1).Save(3);
        }

        [Fact]
        public async Task Restart_connection_on_connection_lost()
        {
            var synchronizer = _container.GetInstance<DomainEventSynchronizer>();

            await synchronizer.StartSynchronization();

            await _inMemoryBus.SimulateConnectionLost();

            _inMemoryBus.IsListening
                .Should()
                .Be(true);
        }

        // ----- Internal logic

        private static ArticleSynchronized SomeArticleSynchronized()
        {
            return new ArticleSynchronized(
                Guid.NewGuid(),
                default,
                default,
                default,
                default,
                default,
                new string[0],
                Guid.NewGuid()
            );
        }
    }
}