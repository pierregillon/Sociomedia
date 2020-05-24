using System;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Medias.Infrastructure;
using StructureMap;

namespace Sociomedia.Medias.Tests.Features
{
    public abstract class AcceptanceTests
    {
        protected readonly ICommandDispatcher CommandDispatcher;
        protected readonly InMemoryEventStore EventStore;
        protected readonly Container Container;

        protected AcceptanceTests()
        {
            Container = new Container(x => x.AddRegistry(new MediasRegistry(new EventStoreConfiguration())));

            Container.Inject<ILogger>(new EmptyLogger());
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