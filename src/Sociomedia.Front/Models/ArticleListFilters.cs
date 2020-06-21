using System;

namespace Sociomedia.Front.Models
{
    public class ArticleListFilters
    {
        public string Search { get; set; }
        public Guid? MediaId { get; set; }
        public Guid? ThemeId { get; set; }
        public static ArticleListFilters None => new ArticleListFilters();

        protected bool Equals(ArticleListFilters other)
        {
            return Search == other.Search && Nullable.Equals(MediaId, other.MediaId) && Nullable.Equals(ThemeId, other.ThemeId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ArticleListFilters) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Search, MediaId, ThemeId);
        }

        public static bool operator== (ArticleListFilters some, ArticleListFilters other)
        {
            return some?.Equals(other) == true;
        }

        public static bool operator !=(ArticleListFilters some, ArticleListFilters other)
        {
            return !(some == other);
        }
    }
}