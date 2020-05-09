using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Sociomedia.Application.Commands.AddMedia;
using Sociomedia.Domain;
using Sociomedia.Domain.Medias;
using Xunit;

namespace Sociomedia.Tests.Features
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

            await CommandDispatcher.Dispatch<AddMediaCommand, Guid>(command);

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new MediaAdded(default, command.Name, command.ImageUrl, command.PoliticalOrientation),
                    new MediaFeedAdded(default, command.Feeds.Single())
                }, x => x.ExcludeDomainEventTechnicalFields());
        }
    }
}