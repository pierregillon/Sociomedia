using System.Threading.Tasks;
using FluentAssertions;
using Sociomedia.DomainEvents.RssSource;
using Sociomedia.FeedAggregator.Application.Commands.AddMedia;
using Xunit;

namespace Sociomedia.FeedAggregator.Tests.Features {
    public class AddMedia : AcceptanceTests
    {
        [Fact]
        public async Task Add_new_source_store_new_added_event()
        {
            var command = new AddMediaCommand("marianne", "https://www.marianne.net/sites/default/themes/marianne/images/logo-marianne.svg", PoliticalOrientation.Left);

            await CommandDispatcher.Dispatch(command);

            (await EventStore.GetAllEvents())
                .Should()
                .BeEquivalentTo(new {
                    Name = command.Name,
                    ImageUrl = command.ImageUrl,
                    PoliticalOrientation = command.PoliticalOrientation
                });
        }
    }
}