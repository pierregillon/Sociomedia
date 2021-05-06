using CQRSlite.Events;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Themes.Application;
using Sociomedia.Themes.Infrastructure;
using StructureMap;

namespace Sociomedia.Tests.AcceptanceTests
{
    public abstract class AcceptanceTests
    {
        protected readonly ICommandDispatcher CommandDispatcher;
        protected readonly Container Container;
        protected readonly InMemoryEventStore EventStore;
        protected readonly IEventPublisher EventPublisher;

        protected AcceptanceTests()
        {
            Container = new Container(x => {
                x.AddRegistry(new CoreRegistry(new EventStoreConfiguration()));
                x.AddRegistry(new ThemesRegistry(new ThemeCalculatorConfiguration { ArticleAggregationIntervalInDays = 30 }));
                x.For<InMemoryEventStore>().Singleton();
            });

            Container.Inject<IEventStore>(Container.GetInstance<InMemoryEventStore>());
            Container.Inject<IEventStoreExtended>(Container.GetInstance<InMemoryEventStore>());
            Container.Inject(Substitute.For<ILogger>());

            CommandDispatcher = Container.GetInstance<ICommandDispatcher>();

            EventStore = Container.GetInstance<InMemoryEventStore>();
            EventPublisher = Container.GetInstance<IEventPublisher>();
        }
    }
}