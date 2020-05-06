using System.Collections.Generic;
using Sociomedia.DomainEvents.Media;

namespace Sociomedia.FeedAggregator.Application.Commands.AddMedia
{
    public class AddMediaCommand : ICommand
    {
        public string Name { get; }
        public string ImageUrl { get; }
        public PoliticalOrientation PoliticalOrientation { get; }
        public IReadOnlyCollection<string> Feeds { get; }

        public AddMediaCommand(string name, string imageUrl, PoliticalOrientation politicalOrientation, IReadOnlyCollection<string> feeds)
        {
            Name = name;
            ImageUrl = imageUrl;
            PoliticalOrientation = politicalOrientation;
            Feeds = feeds;
        }
    }
}
