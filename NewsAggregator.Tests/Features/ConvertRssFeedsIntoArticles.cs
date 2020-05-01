using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NewsAggregator.Application.SynchronizeRssFeed;
using NewsAggregator.Domain;
using NewsAggregator.Domain.Rss;
using NewsAggregator.Infrastructure.CQRS;
using NSubstitute;
using Xunit;

namespace NewsAggregator.Tests.Features
{
    public class ConvertRssFeedsIntoArticles
    {
        private readonly IRssSourceRepository _rssSourceRepository = Substitute.For<IRssSourceRepository>();
        private readonly IRssFeedReader _rssFeedReader = Substitute.For<IRssFeedReader>();
        private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
        private readonly ICommandDispatcher _commandDispatcher;
        private IReadOnlyCollection<IDomainEvent> _publishedDomainEvents = new List<IDomainEvent>();

        public ConvertRssFeedsIntoArticles()
        {
            var container = ContainerBuilder.Build();
            container.Inject(_rssSourceRepository);
            container.Inject(_rssFeedReader);
            container.Inject(_eventPublisher);
            _commandDispatcher = container.GetInstance<ICommandDispatcher>();

            _eventPublisher
                .When(x => x.Publish(Arg.Any<IReadOnlyCollection<IDomainEvent>>()))
                .Do(x => _publishedDomainEvents = (IReadOnlyCollection<IDomainEvent>) x[0]);
        }


        [Fact]
        public async Task Do_not_create_any_articles_when_no_new_rss_feeds()
        {
            var rssSource = new RssSource { Id = "Test", LastSynchronizationDate = new DateTime(2020, 05, 01) };

            _rssSourceRepository
                .GetAll()
                .Returns(x => { return new[] { rssSource }; });

            _rssFeedReader
                .Read(rssSource)
                .Returns(x => new RssFeeds(new[] {
                    new RssFeed { Id = "Test1", Url = "https://www.test.com/rss.xml", PublishDate = new DateTime(2020, 01, 01) }
                }));

            await _commandDispatcher.Dispatch(new SynchronizeRssFeedCommand());

            _publishedDomainEvents
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task Create_new_articles_when_new_feed()
        {
            var rssSource = new RssSource { Id = "Test", LastSynchronizationDate = new DateTime(2020, 05, 01) };

            _rssSourceRepository
                .GetAll()
                .Returns(x => { return new[] { rssSource }; });

            _rssFeedReader
                .Read(rssSource)
                .Returns(x => new RssFeeds(new[] {
                    new RssFeed { Id = "Test1", Url = "https://www.test.com/rss.xml", PublishDate = new DateTime(2020, 05, 02), Html = "" }
                }));

            await _commandDispatcher.Dispatch(new SynchronizeRssFeedCommand());

            _publishedDomainEvents
                .Should()
                .BeEquivalentTo(new[] {
                    new { Name = "https://www.test.com/rss.xml", Keywords = new Keyword[0] }
                });
        }
    }
}