using System;
using CQRSlite.Events;
using EventStore.ClientAPI;
using NSubstitute;
using Sociomedia.Domain.Articles;
using Sociomedia.Infrastructure;
using Sociomedia.Infrastructure.CQRS;
using StructureMap;

namespace Sociomedia.Tests.Features
{
    public abstract class AcceptanceTests
    {
        protected readonly ICommandDispatcher CommandDispatcher;
        protected readonly InMemoryEventStore EventStore;
        protected readonly Container Container;

        protected AcceptanceTests()
        {
            Container = new Container(x => x.AddRegistry<SociomediaRegistry>());

            Container.Inject<ILogger>(new EmptyLogger());
            Container.Inject(Substitute.For<IHtmlPageDownloader>());
            Container.Inject<IEventStore>(Container.GetInstance<InMemoryEventStore>());

            CommandDispatcher = Container.GetInstance<ICommandDispatcher>();
            EventStore = (InMemoryEventStore) Container.GetInstance<IEventStore>();
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