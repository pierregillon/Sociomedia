using System;
using System.Threading.Tasks;
using CQRSlite.Domain;
using NewsAggregator.Domain.Rss;

namespace NewsAggregator.Application.Commands.AddRssSource
{
    public class AddRssSourceCommand : ICommand
    {
        public Uri Url { get; }

        public AddRssSourceCommand(Uri url)
        {
            Url = url;
        }
    }

    public class AddRssSourceCommandHandler : ICommandHandler<AddRssSourceCommand>
    {
        private readonly IRepository _repository;

        public AddRssSourceCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(AddRssSourceCommand command)
        {
            var rssSource = new RssSource(command.Url);

            await _repository.Save(rssSource);
        }
    }
}
