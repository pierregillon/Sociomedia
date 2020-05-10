using System;
using CQRSlite.Events;

namespace Sociomedia.Domain.Medias {
    public class MediaEdited : MediaEvent
    {
        public string Name { get; }
        public string ImageUrl { get; }
        public PoliticalOrientation PoliticalOrientation { get; }

        public MediaEdited(Guid id, string name, string imageUrl, PoliticalOrientation politicalOrientation) : base(id)
        {
            Name = name;
            ImageUrl = imageUrl;
            PoliticalOrientation = politicalOrientation;
        }
    }
}