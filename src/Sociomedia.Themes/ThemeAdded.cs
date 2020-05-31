using System;
using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.Themes.Domain
{
    public class ThemeAdded : ThemeEvent
    {
        public IReadOnlyCollection<Keyword2> Keywords { get; }
        public IReadOnlyCollection<Guid> Articles { get; }

        public ThemeAdded(Guid id, IReadOnlyCollection<Keyword2> keywords, IReadOnlyCollection<Guid> articles) : base(id)
        {
            Id = id;
            Keywords = keywords;
            Articles = articles;
        }

        protected bool Equals(ThemeAdded other)
        {
            return Keywords.Select(x=>x.Value).SequenceEqual(other.Keywords.Select(x => x.Value)) && Articles.SequenceEqual(other.Articles);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ThemeAdded) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 19;
                foreach (var keyword in Keywords)
                {
                    hash = hash * 31 + keyword.Value.GetHashCode();
                }
                foreach (var article in Articles)
                {
                    hash = hash * 31 + article.GetHashCode();
                }
                return hash;
            }
        }
    }
}