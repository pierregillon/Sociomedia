using System;
using System.Threading.Tasks;
using FluentAssertions;
using Sociomedia.FeedAggregator.Application.Commands.AddRssSource;
using Xunit;

namespace Sociomedia.FeedAggregator.Tests.Features
{
    public class AddRssSource : AcceptanceTests
    {
        [Fact]
        public async Task Add_new_source_store_new_added_event()
        {
            await CommandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("https://www.lemonde.fr/rss/une.xml")));

            (await EventStore.GetAllEvents())
                .Should()
                .BeEquivalentTo(new {
                    Url = new Uri("https://www.lemonde.fr/rss/une.xml")
                });
        }

        [Fact]
        public async Task Add_new_source_store_list_it_correctly()
        {
            await CommandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("https://www.lemonde.fr/rss/une.xml")));

            var list = await RssSourceFinder.GetAll();

            list.Should().BeEquivalentTo(new[] {
                new {
                    LastSynchronizationDate = (DateTimeOffset?) null,
                    Url = new Uri("https://www.lemonde.fr/rss/une.xml")
                }
            });
        }
    }
}