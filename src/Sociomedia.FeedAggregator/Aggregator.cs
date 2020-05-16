using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Sociomedia.Articles.Application.Commands.SynchronizeAllMediaFeeds;
using Sociomedia.Articles.Domain;
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

        public Aggregator(
            IEventBus eventBus,
            Configuration configuration,
            ICommandDispatcher commandDispatcher,
            IEventPublisher eventPublisher,
            ILogger logger)
        {
            _eventBus = eventBus;
            _configuration = configuration;
            _commandDispatcher = commandDispatcher;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public Task StartAggregation(CancellationToken token)
        {
            return Task.Factory.StartNew(async () => { await Process(token); }, token);
        }

        private async Task Process(CancellationToken token)
        {
            await _eventBus.SubscribeToEvents(null, GetEventTypes(), DomainEventReceived);

            try {
                do {
                    await Task.Delay(_configuration.FeedAggregator.SynchronizationTimespan, token);

                    if (_eventBus.IsConnected) {
                        await _commandDispatcher.Dispatch(new SynchronizeAllMediaFeedsCommand());
                    }
                } while (true);
            }
            catch (TaskCanceledException) { }
            catch (Exception ex) {
                Error(ex);
            }
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

        private void Error(Exception ex)
        {
            _logger.Error(ex, "[AGGREGATOR] Unhandled exception : ");
        }

    }
}