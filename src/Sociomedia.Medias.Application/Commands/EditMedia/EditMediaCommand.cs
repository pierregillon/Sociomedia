﻿using System;
using System.Collections.Generic;
using Sociomedia.Core.Application;
using Sociomedia.Medias.Domain;

namespace Sociomedia.Medias.Application.Commands.EditMedia {
    public class EditMediaCommand : ICommand
    {
        public Guid MediaId { get; }
        public string Name { get; }
        public string ImageUrl { get; }
        public PoliticalOrientation PoliticalOrientation { get; }
        public IReadOnlyCollection<string> Feeds { get; }

        public EditMediaCommand(Guid mediaId, string name, string imageUrl, PoliticalOrientation politicalOrientation, IReadOnlyCollection<string> feeds)
        {
            MediaId = mediaId;
            Name = name;
            ImageUrl = imageUrl;
            PoliticalOrientation = politicalOrientation;
            Feeds = feeds;
        }
    }
}