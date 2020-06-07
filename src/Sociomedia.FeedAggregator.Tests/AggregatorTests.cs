using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Sociomedia.Core.Application;
using Sociomedia.Core.Domain;
using Sociomedia.FeedAggregator.Application;
using Xunit;

namespace Sociomedia.FeedAggregator.Tests
{
    public class AggregatorTests
    {
        private readonly Aggregator _aggregator;
        private readonly IEventPositionRepository _eventPositionRepository = Substitute.For<IEventPositionRepository>();
        private readonly IEventStoreExtended _eventStore = Substitute.For<IEventStoreExtended>();

        public AggregatorTests()
        {
            var container = ContainerBuilder.Build(new Configuration());

            container.Inject(_eventPositionRepository);
            container.Inject(_eventStore);

            _aggregator = container.GetInstance<Aggregator>();
        }

        [Fact]
        public async Task Initialize_event_position_to_event_store_current_global_stream_position_when_first_time()
        {
            const long currentPosition = 1500000L;

            _eventPositionRepository.GetLastProcessedPosition().Returns((long?) null);
            _eventStore.GetCurrentGlobalStreamPosition().Returns(currentPosition);

            await _aggregator.StartAggregation(new CancellationToken(false));

            await _eventPositionRepository.Received(1).Save(currentPosition);
        }
    }
}