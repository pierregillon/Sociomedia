using System;
using System.Threading.Tasks;
using CQRSlite.Events;
using FluentAssertions;
using NewsAggregator.Application.Commands.AddRssSource;
using NewsAggregator.Application.Queries;
using NewsAggregator.Infrastructure;
using NewsAggregator.Infrastructure.CQRS;
using Xunit;

namespace NewsAggregator.Tests.Features
{
    public class AddRssSource
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly InMemoryEventStore _eventStore;
        private IRssSourceFinder _rssSourceFinder;

        public AddRssSource()
        {
            var container = ContainerBuilder.Build();

            _rssSourceFinder = container.GetInstance<IRssSourceFinder>();
            _commandDispatcher = container.GetInstance<ICommandDispatcher>();
            _eventStore = (InMemoryEventStore) container.GetInstance<IEventStore>();
        }


        [Fact]
        public async Task Add_new_source_store_new_added_event()
        {
            await _commandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("https://www.lemonde.fr/rss/une.xml")));

            (await _eventStore.GetAllEvents())
                .Should()
                .BeEquivalentTo(new {
                    Url = new Uri("https://www.lemonde.fr/rss/une.xml")
                });
        }

        [Fact]
        public async Task Add_new_source_store_list_it_correctly()
        {
            await _commandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("https://www.lemonde.fr/rss/une.xml")));

            var list = await _rssSourceFinder.GetAll();

            list.Should().BeEquivalentTo(new[] {
                new {
                    LastSynchronizationDate = (DateTimeOffset?)null,
                    Url = new Uri("https://www.lemonde.fr/rss/une.xml")
                }
            });
        }
    }
}