using System;
using CQRSlite.Events;
using EventStore.ClientAPI;
using NSubstitute;
using Sociomedia.Domain.Articles;
using Sociomedia.FeedAggregator;
using Sociomedia.FeedAggregator.Application.Queries;
using Sociomedia.Infrastructure;
using Sociomedia.Infrastructure.CQRS;
using StructureMap;

namespace FeedAggregator.Tests
{
    public abstract class AcceptanceTests
    {
        protected readonly ICommandDispatcher CommandDispatcher;
        protected readonly InMemoryEventStore EventStore;
        protected readonly IMediaFeedFinder MediaFeedFinder;
        protected readonly Container Container;
        private readonly IWebPageDownloader _webPageDownloader = Substitute.For<IWebPageDownloader>();

        protected AcceptanceTests()
        {
            Container = ContainerBuilder.Build(new Configuration());

            Container.Inject<ILogger>(new EmptyLogger());
            Container.Inject(_webPageDownloader);
            Container.Inject<IEventStore>(Container.GetInstance<InMemoryEventStore>());

            MediaFeedFinder = Container.GetInstance<IMediaFeedFinder>();
            CommandDispatcher = Container.GetInstance<ICommandDispatcher>();
            EventStore = (InMemoryEventStore) Container.GetInstance<IEventStore>();

            _webPageDownloader.Download(Arg.Any<Uri>()).Returns("<html>bla</html>");
        }

        private class EmptyLogger : ILogger
        {
            public void Error(string format, params object[] args) { }

            public void Error(Exception ex, string format, params object[] args) { }

            public void Info(string format, params object[] args) { }

            public void Info(Exception ex, string format, params object[] args) { }

            public void Debug(string format, params object[] args) { }

            public void Debug(Exception ex, string format, params object[] args) { }
        }
    }
}