using System;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Core.Infrastructure.EventStoring;
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
                x.AddRegistry<ThemesRegistry>();
                x.For<InMemoryEventStore>().Singleton();
            });

            Container.Inject<IEventStore>(Container.GetInstance<InMemoryEventStore>());
            Container.Inject<IEventStoreExtended>(Container.GetInstance<InMemoryEventStore>());
            Container.Inject<ILogger>(new EmptyLogger());

            CommandDispatcher = Container.GetInstance<ICommandDispatcher>();

            EventStore = Container.GetInstance<InMemoryEventStore>();
            EventPublisher = Container.GetInstance<IEventPublisher>();
        }

        public class EmptyLogger : ILogger
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