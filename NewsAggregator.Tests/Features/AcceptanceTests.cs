using System.Threading.Tasks;
using CQRSlite.Events;
using NewsAggregator.Application.Queries;
using NewsAggregator.Domain.Articles;
using NewsAggregator.Infrastructure;
using NewsAggregator.Infrastructure.CQRS;
using NewsAggregator.Infrastructure.Logging;
using NSubstitute;
using StructureMap;

namespace NewsAggregator.Tests.Features
{
    public abstract class AcceptanceTests
    {
        protected readonly ICommandDispatcher CommandDispatcher;
        protected readonly InMemoryEventStore EventStore;
        protected readonly IRssSourceFinder RssSourceFinder;
        protected readonly Container Container;

        protected AcceptanceTests()
        {
            Container = ContainerBuilder.Build();

            Container.Inject<ILogger>(new EmptyLogger());
            Container.Inject(Substitute.For<IHtmlPageDownloader>());

            RssSourceFinder = Container.GetInstance<IRssSourceFinder>();
            CommandDispatcher = Container.GetInstance<ICommandDispatcher>();
            EventStore = (InMemoryEventStore) Container.GetInstance<IEventStore>();
        }

        private class EmptyLogger : ILogger
        {
            public Task LogInformation(string message, long? elapsedMilliseconds = null)
            {
                return Task.CompletedTask;
            }
        }
    }
}