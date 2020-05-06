using System;

namespace Sociomedia.Front.Data
{
    public class MediaListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string PoliticalOrientation { get; set; }
    }
}