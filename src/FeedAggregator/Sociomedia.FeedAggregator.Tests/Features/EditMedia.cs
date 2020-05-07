using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Sociomedia.Application.Commands.AddMedia;
using Sociomedia.Application.Commands.EditMedia;
using Sociomedia.DomainEvents;
using Sociomedia.DomainEvents.Media;
using Xunit;

namespace Sociomedia.Tests.Features
{
    public class EditMedia : AcceptanceTests
    {
        [Fact]
        public async Task Edit_media_store_media_edited_event()
        {
            // Arrange
            var mediaId = await CommandDispatcher.Dispatch<AddMediaCommand, Guid>(new AddMediaCommand(
                "marianne",
                "https://www.marianne.net/sites/default/themes/marianne/images/logo-marianne.svg",
                PoliticalOrientation.Left,
                new string[0]
            ));
            EventStore.CommitEvents();

            // Act
            var command = new EditMediaCommand(
                mediaId,
                "marianne 2",
                "https://www.marianne.net/sites/default/themes/marianne/images/logo-marianne2.svg",
                PoliticalOrientation.ExtremeLeft,
                new string[0]
            );
            await CommandDispatcher.Dispatch(command);

            // Assert
            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new MediaEdited(default, command.Name, command.ImageUrl, command.PoliticalOrientation)
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Edit_media_feeds_store_feed_events()
        {
            // Arrange
            var addCommand = new AddMediaCommand(
                "marianne",
                "https://www.marianne.net/sites/default/themes/marianne/images/logo-marianne.svg",
                PoliticalOrientation.Left,
                new[] {
                    "https://www.marianne.net/rss_marianne.xml"
                }
            );
            var mediaId = await CommandDispatcher.Dispatch<AddMediaCommand, Guid>(addCommand);
            EventStore.CommitEvents();

            // Act
            var command = new EditMediaCommand(
                mediaId,
                addCommand.Name,
                addCommand.ImageUrl,
                addCommand.PoliticalOrientation,
                new[] {
                    "https://www.marianne.net/rss_marianne2.xml",
                    "https://www.marianne.net/rss_marianne3.xml",
                }
            );
            await CommandDispatcher.Dispatch(command);

            // Assert
            var events = (await EventStore.GetNewEvents()).Skip(1);

            events.Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new MediaFeedRemoved(default, "https://www.marianne.net/rss_marianne.xml"),
                    new MediaFeedRemoved(default, "https://www.marianne.net/rss_marianne2.xml"),
                    new MediaFeedAdded(default, "https://www.marianne.net/rss_marianne3.xml")
                }, x => x.ExcludeDomainEventTechnicalFields());
        }
    }
}