using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using Microsoft.Extensions.Logging;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core;
using Sociomedia.Core.Application;
using Sociomedia.Core.Application.Projections;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Medias.Domain;

namespace Sociomedia.FeedAggregator.Application
{
    public class Aggregator
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStoreExtended _eventStore;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEventPositionRepository _eventPositionRepository;
        private readonly ILogger _logger;
        private readonly ProjectionsBootstrapper _projectionsBootstrapper;
        private readonly FeedsSynchronizer _feedsSynchronizer;

        public Aggregator(
            IEventBus eventBus,
            IEventStoreExtended eventStore,
            IEventPublisher eventPublisher,
            IEventPositionRepository eventPositionRepository,
            ILogger logger,
            FeedsSynchronizer feedsSynchronizer,
            ProjectionsBootstrapper projectionsBootstrapper)
        {
            _eventBus = eventBus;
            _feedsSynchronizer = feedsSynchronizer;
            _eventPublisher = eventPublisher;
            _logger = logger;
            _projectionsBootstrapper = projectionsBootstrapper;
            _eventPositionRepository = eventPositionRepository;
            _eventStore = eventStore;
        }

        // ----- Public methods

        public async Task StartAggregation(CancellationToken token)
        {
            var lastStreamEventPosition = await GetLastStreamEventPosition();
            await _projectionsBootstrapper.InitializeUntil(lastStreamEventPosition.Value, typeof(ArticleEvent));
            await _eventBus.SubscribeToEvents(lastStreamEventPosition, GetEventTypes(), DomainEventReceived);
            StartFeedSynchronization(token);
        }

        // ----- Call backs

        private async Task DomainEventReceived(IEvent @event, long position)
        {
            await _eventPublisher.Publish(@event);
            await _eventPositionRepository.Save(position);
        }

        private void StartFeedSynchronization(CancellationToken token)
        {
            Task.Factory.StartNew(async () => {
                try {
                    await _feedsSynchronizer.PeriodicallySynchronizeFeeds(token);
                }
                catch (Exception ex) {
                    Error(ex);
                }
            }, token);
        }

        // ----- Private

        private async Task<long?> GetLastStreamEventPosition()
        {
            var lastPosition = await _eventPositionRepository.GetLastProcessedPosition();
            if (!lastPosition.HasValue) {
                lastPosition = await _eventStore.GetCurrentGlobalStreamPosition();
                await _eventPositionRepository.Save(lastPosition.Value);
                Info($"No last position found, initializing the last position to the current event store global stream position : {lastPosition}");
            }
            return lastPosition;
        }

        private static IEnumerable<Type> GetEventTypes()
        {
            return typeof(MediaEvent).Assembly.GetTypes()
                .Where(x => x.IsDomainEvent())
                .ToArray();
        }

        private void Error(Exception ex)
        {
            _logger.LogError(ex, $"[{GetType().DisplayableName()}] Unhandled exception : ");
        }

        private void Info(string message)
        {
            _logger.LogInformation($"[{GetType().DisplayableName()}] " + message);
        }
    }
}