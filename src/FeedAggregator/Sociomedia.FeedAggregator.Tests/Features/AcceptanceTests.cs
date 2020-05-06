using System;
using System.Threading.Tasks;
using CQRSlite.Events;
using NSubstitute;
using Sociomedia.FeedAggregator.Application.Queries;
using Sociomedia.FeedAggregator.Domain.Articles;
using Sociomedia.FeedAggregator.Infrastructure;
using Sociomedia.FeedAggregator.Infrastructure.CQRS;
using Sociomedia.FeedAggregator.Infrastructure.Logging;
using StructureMap;

namespace Sociomedia.FeedAggregator.Tests.Features
{
    public abstract class AcceptanceTests
    {
        protected readonly ICommandDispatcher CommandDispatcher;
        protected readonly InMemoryEventStore EventStore;
        protected readonly IMediaFeedFinder MediaFeedFinder;
        protected readonly Container Container;

        protected AcceptanceTests()
        {
            Container = ContainerBuilder.Build();

            Container.Inject<ILogger>(new EmptyLogger());
            Container.Inject(Substitute.For<IHtmlPageDownloader>());
            Container.Inject<IEventStore>(Container.GetInstance<InMemoryEventStore>());

            MediaFeedFinder = Container.GetInstance<IMediaFeedFinder>();
            CommandDispatcher = Container.GetInstance<ICommandDispatcher>();
            EventStore = (InMemoryEventStore) Container.GetInstance<IEventStore>();
        }

        private class EmptyLogger : ILogger
        {
            public Task LogInformation(string message, long? elapsedMilliseconds = null)
            {
                return Task.CompletedTask;
            }

            public Task LogError(Exception ex)
            {
                return Task.CompletedTask;
            }
        }
    }
}