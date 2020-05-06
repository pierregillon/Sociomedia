using System.ComponentModel.DataAnnotations;
using Sociomedia.DomainEvents.RssSource;

namespace Sociomedia.Front.Models
{
    public class ArticleViewModel
    {
        [Required] public string Name { get; set; }
        public string ImageUrl { get; set; }
        [Required] public PoliticalOrientation PoliticalOrientation { get; set; }
    }
}