using System.ComponentModel.DataAnnotations;

namespace Sociomedia.Front.Models
{
    public class ArticleViewModel
    {
        [Required] public string Name { get; set; }
        public string ImageUrl { get; set; }
        [Required] public PoliticalOrientation PoliticalOrientation { get; set; }
    }

    public enum PoliticalOrientation
    {
        ExtremeLeft,
        Left,
        Center,
        Right,
        ExtremeRight
    }
}