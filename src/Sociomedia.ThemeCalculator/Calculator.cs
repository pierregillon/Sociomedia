using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core;
using Sociomedia.Core.Application;
using Sociomedia.Core.Application.Projections;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.EventStoring;

namespace Sociomedia.ThemeCalculator
{
    public class Calculator
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStoreExtended _eventStore;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEventPositionRepository _eventPositionRepository;
        private readonly ILogger _logger;
        private readonly ProjectionsBootstrapper _projectionsBootstrapper;

        public Calculator(
            IEventBus eventBus,
            IEventStoreExtended eventStore,
            IEventPublisher eventPublisher,
            IEventPositionRepository eventPositionRepository,
            ILogger logger,
            ProjectionsBootstrapper projectionsBootstrapper)
        {
            _eventBus = eventBus;
            _eventPublisher = eventPublisher;
            _logger = logger;
            _projectionsBootstrapper = projectionsBootstrapper;
            _eventPositionRepository = eventPositionRepository;
            _eventStore = eventStore;
        }

        // ----- Public methods

        public async Task StartCalculation(CancellationToken token)
        {
            var lastStreamEventPosition = await GetLastStreamEventPosition();
            if (lastStreamEventPosition.HasValue) {
                await _projectionsBootstrapper.InitializeUntil(lastStreamEventPosition.Value);
            }
            await _eventBus.SubscribeToEvents(lastStreamEventPosition, GetEventTypes(), DomainEventReceived);
            await Task.Delay(-1, token);
        }

        // ----- Call backs

        private async Task DomainEventReceived(IEvent @event, long position)
        {
            await _eventPublisher.Publish(@event);
            await _eventPositionRepository.Save(position);
        }

        // ----- Private

        private async Task<long?> GetLastStreamEventPosition()
        {
            var lastPosition = await _eventPositionRepository.GetLastProcessedPosition();
            if (!lastPosition.HasValue) {
                Info("No last position in stream found");
            }
            return lastPosition;
        }

        private static IEnumerable<Type> GetEventTypes()
        {
            return typeof(ArticleEvent).Assembly.GetTypes()
                .Where(x => x.IsDomainEvent())
                .ToArray();
        }

        private void Info(string message)
        {
            _logger.Info($"[{GetType().DisplayableName()}] " + message);
        }
    }
}