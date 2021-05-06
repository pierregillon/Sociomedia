using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sociomedia.Core.Domain;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public class EventStoreOrg : IEventStore, IEventStoreExtended
    {
        private readonly EventStoreClient _client;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ITypeLocator _typeLocator;

        public EventStoreOrg(EventStoreConfiguration configuration, ILogger logger, ITypeLocator typeLocator)
        {
            _typeLocator = typeLocator;

            var jsonResolver = new PropertyCleanerSerializerContractResolver();
            jsonResolver.IgnoreProperty(typeof(IEvent), "Version");
            jsonResolver.RenameProperty(typeof(IEvent), "Id", "AggregateId");

            _serializerSettings = new JsonSerializerSettings {
                ContractResolver = jsonResolver,
                Formatting = Formatting.Indented
            };

            var settings = EventStoreClientSettings.Create(configuration.ConnectionString);
            settings.ConnectionName = AppDomain.CurrentDomain.FriendlyName;
            _client = new EventStoreClient(settings);
        }

        // ----- Public methods

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            var streamEvents = await ReadAllEventsInStream(aggregateId.ToString(), fromVersion);

            return streamEvents
                .Select(ConvertToDomainEvent)
                .ToArray();
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
        {
            var allEvents = events.ToArray();
            if (!allEvents.Any()) {
                throw new InvalidOperationException("No events to save !");
            }

            EventData ConvertToEventData(IEvent @event)
            {
                var json = JsonConvert.SerializeObject(@event, _serializerSettings);
                return new EventData(
                    Uuid.NewUuid(),
                    @event.GetType().Name,
                    Encoding.UTF8.GetBytes(json)
                );
            }

            var firstEvent = allEvents.First();
            var version = new StreamRevision((ulong) firstEvent.Version - 2); // CQRSLite start event version at 1. EventStore at -1.
            var streamName = firstEvent.Id.ToString();
            await _client.AppendToStreamAsync(streamName, version, allEvents.Select(ConvertToEventData), cancellationToken: cancellationToken);
        }

        public async IAsyncEnumerable<IEvent> GetAllEventsBetween(Position startPosition, Position endPosition, IReadOnlyCollection<Type> eventTypes)
        {
            var eventTypesByName = eventTypes.ToDictionary(x => x.Name);

            await foreach (var @event in _client.ReadAllAsync(Direction.Forwards, startPosition)) {
                if (@event.Event.EventType.StartsWith("$")) {
                    continue;
                }
                if (eventTypesByName.TryGetValue(@event.Event.EventType, out var eventType)) {
                    yield return ConvertToDomainEvent(@event, eventType);
                }
                if (@event.OriginalPosition == endPosition) {
                    yield break;
                }
            }
        }

        public async Task<long> GetCurrentGlobalStreamPosition()
        {
            var @event = await FirstOrDefault(_client.ReadAllAsync(Direction.Backwards, Position.End, 1));
            if (!@event.HasValue) {
                return 0;
            }
            if (@event.Value.OriginalPosition.HasValue) {
                return (long) @event.Value.OriginalPosition.Value.CommitPosition;
            }
            return 0;
        }

        private static async Task<ResolvedEvent?> FirstOrDefault(IAsyncEnumerable<ResolvedEvent> enumerable)
        {
            await foreach (var test in enumerable) {
                return test;
            }
            return default;
        }

        // ----- Internal logic

        private IEvent ConvertToDomainEvent(ResolvedEvent @event)
        {
            try {
                var type = _typeLocator.FindEventType(@event.Event.EventType);
                if (type == null) {
                    throw new Exception("Event is unknown, unable to correctly deserialize it.");
                }
                return ConvertToDomainEvent(@event, type);
            }
            catch (Exception ex) {
                throw new Exception($"An error occurred while parsing event from event store. Stream: {@event.Event.EventStreamId}, Position: {@event.Event.EventNumber}", ex);
            }
        }

        private IEvent ConvertToDomainEvent(ResolvedEvent @event, Type eventType)
        {
            var json = Encoding.UTF8.GetString(@event.Event.Data.ToArray());
            var domainEvent = (IEvent) JsonConvert.DeserializeObject(json, eventType, _serializerSettings);
            domainEvent.Version = (int) @event.OriginalEventNumber.ToInt64() + 1;
            return domainEvent;
        }

        private async Task<IReadOnlyCollection<ResolvedEvent>> ReadAllEventsInStream(string streamId, int fromVersion)
        {
            var position = fromVersion == -1 ? StreamPosition.Start : new StreamPosition((ulong) fromVersion);

            var result = _client.ReadStreamAsync(Direction.Forwards, streamId, position);

            if (await result.ReadState == ReadState.StreamNotFound) {
                return Array.Empty<ResolvedEvent>();
            }

            return await _client.ReadStreamAsync(Direction.Forwards, streamId, position).EnumerateAsync();
        }
    }
}