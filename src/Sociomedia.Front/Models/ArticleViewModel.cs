using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sociomedia.Medias.Domain;

namespace Sociomedia.Front.Models
{
    public class ArticleViewModel
    {
        [Required] public string Name { get; set; }

        public string ImageUrl { get; set; }

        [Required] public PoliticalOrientation PoliticalOrientation { get; set; }

        public List<FeedItem> Feeds { get; set; } = new List<FeedItem>();
        public Guid Id { get; set; }

        public ArticleViewModel()
        {
            AddFeed();
        }

        public void AddFeed()
        {
            Feeds.Add(new FeedItem {
                Id = Feeds.Count + 1
            });
        }
    }

    public class FeedItem
    {
        public int Id { get; set; }
        [Required] public string Url { get; set; }
    }
}