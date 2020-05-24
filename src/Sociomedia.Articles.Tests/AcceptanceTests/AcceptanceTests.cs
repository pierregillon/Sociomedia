using System;
using CQRSlite.Events;
using EventStore.ClientAPI;
using NSubstitute;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Keywords;
using Sociomedia.Articles.Infrastructure;
using Sociomedia.Core.Domain;
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

        protected AcceptanceTests()
        {
            Container = new Container(x => {
                x.AddRegistry(new ArticlesRegistry(new EventStoreConfiguration()));
                x.For<InMemoryEventStore>().Singleton();
            });


            Container.Inject(Substitute.For<IWebPageDownloader>());
            Container.Inject(Substitute.For<IKeywordDictionary>());
            Container.Inject<IEventStore>(Container.GetInstance<InMemoryEventStore>());
            Container.Inject<IEventStoreExtended>(Container.GetInstance<InMemoryEventStore>());
            Container.Inject<ILogger>(new EmptyLogger());

            Container
                .GetInstance<IKeywordDictionary>()
                .IsValidKeyword(Arg.Any<string>())
                .Returns(true);

            CommandDispatcher = Container.GetInstance<ICommandDispatcher>();
            WebPageDownloader = Container.GetInstance<IWebPageDownloader>();
            EventStore = (InMemoryEventStore) Container.GetInstance<IEventStore>();

            WebPageDownloader
                .Download(Arg.Any<string>())
                .Returns("<html></html>");
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