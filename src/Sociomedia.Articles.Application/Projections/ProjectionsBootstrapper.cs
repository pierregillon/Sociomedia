using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.ClientAPI;
using Sociomedia.Articles.Domain;
using Sociomedia.Core;
using Sociomedia.Core.Application;
using Sociomedia.Core.Domain;

namespace Sociomedia.Articles.Application.Projections
{
    public class ProjectionsBootstrapper
    {
        private readonly IEventStoreExtended _eventStore;
        private readonly IProjectionLocator _projectionLocator;
        private readonly ILogger _logger;

        public ProjectionsBootstrapper(IEventStoreExtended eventStore, IProjectionLocator projectionLocator, ILogger logger)
        {
            _eventStore = eventStore;
            _projectionLocator = projectionLocator;
            _logger = logger;
        }

        public async Task InitializeUntil(long lastStreamPosition)
        {
            Info($"Updating projections to {lastStreamPosition} event position ...");

            IDictionary<Type, List<IProjection>> projectionsByType = new Dictionary<Type, List<IProjection>>();
            foreach (var projection in _projectionLocator.FindProjections()) {
                AddProjectionEvents(projectionsByType, projection);
            }

            var eventCount = 0;

            await foreach (var @event in _eventStore.GetAllEventsBetween(Position.Start, new Position(lastStreamPosition, lastStreamPosition), projectionsByType.Keys.ToArray())) {
                await ApplyInProjections(projectionsByType, @event);
                eventCount++;
            }

            Info($"Projections up-to-date ({eventCount} events replayed).");
        }

        private static async Task ApplyInProjections(IDictionary<Type, List<IProjection>> projectionsByEventType, IEvent @event)
        {
            if (projectionsByEventType.TryGetValue(@event.GetType(), out var projections)) {
                foreach (var projection in projections) {
                    await projection.On(@event);
                }
            }
        }

        private static void AddProjectionEvents(IDictionary<Type, List<IProjection>> projectionsByEventType, IProjection projection)
        {
            var interfaces = projection.GetType().GetInterfaces().Where(x => x.Name == typeof(IEventListener<>).Name);
            foreach (var @interface in interfaces) {
                var eventType = @interface.GetGenericArguments()[0];
                if (projectionsByEventType.TryGetValue(eventType, out var projections)) {
                    projections.Add(projection);
                }
                else {
                    projectionsByEventType.Add(eventType, new List<IProjection> { projection });
                }
            }
        }

        private void Info(string message)
        {
            _logger.Info($"[{this.GetType().Name.SeparatePascalCaseWords().ToUpper()}] " + message);
        }
    }
}