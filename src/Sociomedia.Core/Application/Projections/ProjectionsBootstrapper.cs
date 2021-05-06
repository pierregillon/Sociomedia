using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using Sociomedia.Core.Domain;
using StructureMap.TypeRules;

namespace Sociomedia.Core.Application.Projections
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

        public async Task InitializeUntil(long lastStreamPosition, Type innerDomainEventType)
        {
            Info($"Updating projections to {lastStreamPosition} event position ...");

            IDictionary<Type, List<ProjectionInfo>> projectionsByType = new Dictionary<Type, List<ProjectionInfo>>();
            foreach (var projection in _projectionLocator.FindProjections()) {
                AddProjectionEvents(projectionsByType, projection);
            }

            var allEvents = await ReadAllEvents(lastStreamPosition, innerDomainEventType, projectionsByType.Keys.ToArray());

            foreach (var @event in allEvents) {
                await ApplyInProjections(projectionsByType, @event);
            }

            Info($"Projections up-to-date ({allEvents.Count} events replayed).");
        }

        private async Task<IReadOnlyCollection<IEvent>> ReadAllEvents(long lastStreamPosition, Type innerDomainEventType, IReadOnlyCollection<Type> eventTypes)
        {
            var innerDomainEventTypes = eventTypes.Where(x => x.CanBeCastTo(innerDomainEventType)).ToArray();
            var outerDomainEventTypes = eventTypes.Where(x => !x.CanBeCastTo(innerDomainEventType)).ToArray();

            var innerDomainEvents = await _eventStore.GetAllEventsBetween(Position.Start, new Position((ulong)lastStreamPosition, (ulong)lastStreamPosition), outerDomainEventTypes).ToListAsync();
            var outerDomainEvents = await _eventStore.GetAllEventsBetween(Position.Start, Position.End, innerDomainEventTypes).ToListAsync();

            return innerDomainEvents
                .Concat(outerDomainEvents)
                .OrderBy(x => x.TimeStamp)
                .ToArray();
        }

        private static async Task ApplyInProjections(IDictionary<Type, List<ProjectionInfo>> projectionsByEventType, IEvent @event)
        {
            if (projectionsByEventType.TryGetValue(@event.GetType(), out var projections)) {
                foreach (var projection in projections) {
                    await projection.On(@event);
                }
            }
        }

        private static void AddProjectionEvents(IDictionary<Type, List<ProjectionInfo>> projectionsByEventType, IProjection projection)
        {
            var interfaces = projection.GetType().GetInterfaces().Where(x => x.Name == typeof(IEventListener<>).Name);
            foreach (var @interface in interfaces) {
                var eventType = @interface.GetGenericArguments()[0];
                if (projectionsByEventType.TryGetValue(eventType, out var projections)) {
                    projections.Add(new ProjectionInfo(projection, eventType));
                }
                else {
                    projectionsByEventType.Add(eventType, new List<ProjectionInfo> { new ProjectionInfo(projection, eventType) });
                }
            }
        }

        private void Info(string message)
        {
            _logger.LogInformation($"[{GetType().DisplayableName()}] " + message);
        }
    }
}