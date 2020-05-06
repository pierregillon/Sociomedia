using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Sociomedia.DomainEvents.Media;
using Sociomedia.FeedAggregator.Application.Commands.AddMedia;
using Xunit;

namespace Sociomedia.FeedAggregator.Tests.Features
{
    public class AddMedia : AcceptanceTests
    {
        [Fact]
        public async Task Add_new_source_store_new_added_event()
        {
            var command = new AddMediaCommand(
                "marianne",
                "https://www.marianne.net/sites/default/themes/marianne/images/logo-marianne.svg",
                PoliticalOrientation.Left,
                new[] {
                    "https://www.marianne.net/rss_marianne.xml"
                }
            );

            await CommandDispatcher.Dispatch(command);

            var events = await EventStore.GetAllEvents();

            events
                .OfType<MediaAdded>()
                .Should()
                .BeEquivalentTo(new {
                    Name = command.Name,
                    ImageUrl = command.ImageUrl,
                    PoliticalOrientation = command.PoliticalOrientation
                });

            events
                .OfType<MediaFeedAdded>()
                .Should()
                .BeEquivalentTo(new {
                    FeedUrl = command.Feeds.Single()
                });
        }
    }
}