using System;
using CQRSlite.Events;
using Microsoft.Extensions.Logging;
using NSubstitute;
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

            Container.Inject(Substitute.For<ILogger>());
            Container.Inject<IEventStore>(Container.GetInstance<InMemoryEventStore>());

            CommandDispatcher = Container.GetInstance<ICommandDispatcher>();
            EventStore = (InMemoryEventStore) Container.GetInstance<IEventStore>();
        }
    }
}