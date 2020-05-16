using System;
using System.Collections.Generic;
using Sociomedia.Application.Application;
using Sociomedia.Medias.Domain;

namespace Sociomedia.Medias.Application.Commands.AddMedia
{
    public class AddMediaCommand : ICommand<Guid>
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
