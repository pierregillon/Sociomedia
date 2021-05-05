﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Events;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.Medias.Domain;
using Sociomedia.Themes.Domain;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public class DomainEventSynchronizer
    {
        private readonly IEventBus _eventBus;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStreamPositionRepository _streamPositionRepository;

        public DomainEventSynchronizer(
            IEventBus eventBus,
            IEventPublisher eventPublisher,
            IStreamPositionRepository streamPositionRepository)
        {
            _eventBus = eventBus;
            _eventPublisher = eventPublisher;
            _streamPositionRepository = streamPositionRepository;
        }

        public async Task StartSynchronization()
        {
            var lastPosition = await _streamPositionRepository.GetLastPosition();
            await _eventBus.SubscribeToEvents(lastPosition, GetEventTypes(), DomainEventReceived);
        }

        private static IEnumerable<Type> GetEventTypes()
        {
            var articlesEvents = typeof(ArticleEvent).Assembly.GetTypes()
                .Where(x => x.IsDomainEvent())
                .ToArray();

            var mediaEvents = typeof(MediaEvent).Assembly.GetTypes()
                .Where(x => x.IsDomainEvent())
                .ToArray();

            var themeEvents = typeof(ThemeEvent).Assembly.GetTypes()
                .Where(x => x.IsDomainEvent())
                .ToArray();

            return articlesEvents.Concat(mediaEvents).Concat(themeEvents).ToArray();
        }

        private async Task DomainEventReceived(IEvent @event, long position)
        {
            await _eventPublisher.Publish(@event);
            await _streamPositionRepository.Save(position);
        }

        public void StopSynchronization()
        {
            _eventBus.Stop();
        }
    }
}