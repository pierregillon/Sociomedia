using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Sociomedia.Core.Domain;
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
                x.AddRegistry(new ThemesRegistry(new EventStoreConfiguration()));

                //x.For<InMemoryEventStore>().Singleton();
                //x.For<IEventStore>().ClearAll().Use(context => context.GetInstance<InMemoryEventStore>()).Singleton();
                //x.For<IEventStoreExtended>().ClearAll().Use(context => context.GetInstance<InMemoryEventStore>()).Singleton();
                //x.For<IEventPublisher>().DecorateAllWith<ChainedEventPublisherDecorator>();
                //x.For<ILogger>().ClearAll().Use<EmptyLogger>();

                x.For<InMemoryEventStore>().Singleton();
                x.For<IEventPublisher>().DecorateAllWith<ChainedEventPublisherDecorator>();
                x.For<IEventStore>().DecorateAllWith<EventStorePublisherDecorator>();
            });

            Container.Inject<IEventStore>(Container.GetInstance<InMemoryEventStore>());
            Container.Inject<IEventStoreExtended>(Container.GetInstance<InMemoryEventStore>());
            Container.Inject<ILogger>(new EmptyLogger());

            CommandDispatcher = Container.GetInstance<ICommandDispatcher>();

            EventStore = Container.GetInstance<InMemoryEventStore>();
            EventPublisher = Container.GetInstance<IEventPublisher>();
        }

        protected async Task PublishEvents(IEnumerable<DomainEvent> events)
        {
            foreach (var @event in events) {
                await EventPublisher.Publish(@event);
            }
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

    public class EventStorePublisherDecorator : IEventStore
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;

        public EventStorePublisherDecorator(IEventStore eventStore, IEventPublisher eventPublisher)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = new CancellationToken())
        {
            var enumeratedEvents = events.ToArray();
            await _eventStore.Save(enumeratedEvents, cancellationToken);
            foreach (var @event in enumeratedEvents) {
                await _eventPublisher.Publish(@event, cancellationToken);
            }
        }

        public Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken())
        {
            return _eventStore.Get(aggregateId, fromVersion, cancellationToken);
        }
    }

    public class ChainedEventPublisherDecorator : IEventPublisher
    {
        private readonly IEventPublisher _eventPublisher;
        private static readonly  ConcurrentQueue<IEvent> _eventQueue = new ConcurrentQueue<IEvent>();
        private static bool _publishing = false;

        public ChainedEventPublisherDecorator(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task Publish<T>(T @event, CancellationToken cancellationToken = new CancellationToken()) where T : class, IEvent
        {
            if (_publishing) {
                _eventQueue.Enqueue(@event);
            }
            else {
                try {
                    _publishing = true;
                    await _eventPublisher.Publish(@event, cancellationToken);
                    while (_eventQueue.Count > 0) {
                        if (_eventQueue.TryDequeue(out var value)) {
                            await _eventPublisher.Publish(value, cancellationToken);
                        }
                        else {
                            await Task.Delay(100, cancellationToken);
                        }
                    }
                }
                finally {
                    _publishing = false;
                }
            }
        }
    }
}