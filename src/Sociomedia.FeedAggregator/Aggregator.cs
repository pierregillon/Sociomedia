using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Sociomedia.Articles.Application.Commands.SynchronizeAllMediaFeeds;
using Sociomedia.Articles.Application.Projections;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Infrastructure;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Medias.Domain;

namespace Sociomedia.FeedAggregator
{
    public class Aggregator
    {
        private readonly IEventBus _eventBus;
        private readonly Configuration _configuration;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;
        private readonly ProjectionsBootstrap _projectionsBootstrap;
        private readonly EventPositionRepository _eventPositionRepository;
        private readonly IEventStoreExtended _eventStore;

        public Aggregator(
            IEventBus eventBus,
            Configuration configuration,
            ICommandDispatcher commandDispatcher,
            IEventPublisher eventPublisher,
            ILogger logger,
            ProjectionsBootstrap projectionsBootstrap,
            EventPositionRepository eventPositionRepository,
            IEventStoreExtended eventStore)
        {
            _eventBus = eventBus;
            _configuration = configuration;
            _commandDispatcher = commandDispatcher;
            _eventPublisher = eventPublisher;
            _logger = logger;
            _projectionsBootstrap = projectionsBootstrap;
            _eventPositionRepository = eventPositionRepository;
            _eventStore = eventStore;
        }

        public Task StartAggregation(CancellationToken token)
        {
            return Task.Factory.StartNew(async () => { await Process(token); }, token);
        }

        private async Task Process(CancellationToken token)
        {
            try {
                var lastEventPosition = await GetLastEventPosition();
                await _projectionsBootstrap.Initialize(lastEventPosition.Value);
                await _eventBus.SubscribeToEvents(lastEventPosition, GetEventTypes(), DomainEventReceived, PositionInStreamChanged);
                await PeriodicallySynchronizeFeeds(token);
            }
            catch (Exception ex) {
                Error(ex);
            }
        }

        private async Task<long?> GetLastEventPosition()
        {
            var lastPosition = await _eventPositionRepository.GetLastProcessedPosition();
            if (!lastPosition.HasValue) {
                lastPosition = await _eventStore.GetCurrentPosition();
                await _eventPositionRepository.Save(lastPosition.Value);
                Info($"No last position found, initializing the last position to the current event store position : {lastPosition}");
            }
            return lastPosition;
        }

        private async Task PeriodicallySynchronizeFeeds(CancellationToken token)
        {
            try {
                do {
                    await Task.Delay(_configuration.FeedAggregator.SynchronizationTimespan, token);

                    if (_eventBus.IsConnected) {
                        await _commandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());
                    }
                } while (true);
            }
            catch (TaskCanceledException) { }
        }

        private static IEnumerable<Type> GetEventTypes()
        {
            var articlesEvents = typeof(ArticleImported).Assembly.GetTypes()
                .Where(x => x.IsDomainEvent())
                .ToArray();

            var mediaEvents = typeof(MediaAdded).Assembly.GetTypes()
                .Where(x => x.IsDomainEvent())
                .ToArray();

            return articlesEvents.Union(mediaEvents).ToArray();
        }

        private async Task DomainEventReceived(IEvent @event)
        {
            await _eventPublisher.Publish(@event);
        }

        private async Task PositionInStreamChanged(long position)
        {
            await _eventPositionRepository.Save(position);
        }

        private void Error(Exception ex)
        {
            _logger.Error(ex, "[AGGREGATOR] Unhandled exception : ");
        }

        private void Info(string message)
        {
            _logger.Info("[AGGREGATOR] " + message);
        }
    }
}