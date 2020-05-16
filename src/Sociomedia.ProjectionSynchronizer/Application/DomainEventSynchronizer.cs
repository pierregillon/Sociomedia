using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using Sociomedia.Application.Domain;
using Sociomedia.Application.Infrastructure.EventStoring;
using Sociomedia.Articles.Domain;
using Sociomedia.Medias.Domain;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public class DomainEventSynchronizer
    {
        private readonly IEventBus _eventBus;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStreamPositionRepository _streamPositionRepository;
        private readonly ProjectionSynchronizationConfiguration _configuration;

        public DomainEventSynchronizer(
            IEventBus eventBus,
            IEventPublisher eventPublisher,
            IStreamPositionRepository streamPositionRepository,
            ProjectionSynchronizationConfiguration configuration)
        {
            _eventBus = eventBus;
            _eventPublisher = eventPublisher;
            _streamPositionRepository = streamPositionRepository;
            _configuration = configuration;
        }

        public async Task StartSynchronization()
        {
            var lastPosition = await _streamPositionRepository.GetLastPosition();
            await _eventBus.SubscribeToEvents(lastPosition, GetEventTypes(), DomainEventReceived, PositionInStreamChanged);
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
            await _streamPositionRepository.Save(position);
        }

        public void StopSynchronization()
        {
            _eventBus.Stop();
        }
    }
}