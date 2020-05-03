using System;

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
}
