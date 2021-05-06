using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRSlite.Events;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Keywords;
using Sociomedia.Articles.Infrastructure;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Core.Infrastructure.EventStoring;
using StructureMap;

namespace Sociomedia.Articles.Tests.AcceptanceTests
{
    public abstract class AcceptanceTests
    {
        protected readonly ICommandDispatcher CommandDispatcher;
        protected readonly InMemoryEventStore EventStore;
        protected readonly Container Container;
        protected readonly IWebPageDownloader WebPageDownloader;
        protected readonly IEventPublisher EventPublisher;

        protected AcceptanceTests()
        {
            Container = new Container(x => {
                x.AddRegistry(new CoreRegistry(new EventStoreConfiguration()));
                x.AddRegistry<ArticlesRegistry>();
                x.For<InMemoryEventStore>().Singleton();
            });

            Container.Inject(Substitute.For<IWebPageDownloader>());
            Container.Inject(Substitute.For<IKeywordDictionary>());
            Container.Inject<IEventStore>(Container.GetInstance<InMemoryEventStore>());
            Container.Inject<IEventStoreExtended>(Container.GetInstance<InMemoryEventStore>());
            Container.Inject(Substitute.For<ILogger>());

            Container
                .GetInstance<IKeywordDictionary>()
                .IsValidKeyword(Arg.Any<string>())
                .Returns(true);

            CommandDispatcher = Container.GetInstance<ICommandDispatcher>();
            WebPageDownloader = Container.GetInstance<IWebPageDownloader>();
            EventStore = Container.GetInstance<InMemoryEventStore>();
            EventPublisher = Container.GetInstance<IEventPublisher>();

            WebPageDownloader
                .Download(Arg.Any<string>())
                .Returns("<html></html>");
        }

        protected async Task StoreAndPublish(IReadOnlyCollection<IEvent> events)
        {
            await EventStore.Store(events);
            EventStore.CommitEvents();
            await EventPublisher.Publish(events);
        }
    }
}