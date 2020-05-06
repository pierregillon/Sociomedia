using Sociomedia.DomainEvents.RssSource;

namespace Sociomedia.FeedAggregator.Application.Commands.AddMedia
{
    public class AddMediaCommand : ICommand
    {
        public string Name { get; }
        public string ImageUrl { get; }
        public PoliticalOrientation PoliticalOrientation { get; }

        public AddMediaCommand(string name, string imageUrl, PoliticalOrientation politicalOrientation)
        {
            Name = name;
            ImageUrl = imageUrl;
            PoliticalOrientation = politicalOrientation;
        }
    }
}
