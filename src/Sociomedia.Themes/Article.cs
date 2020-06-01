using System;
using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.Themes.Domain
{
    public class Article
    {
        public Guid Id { get; }
        public IReadOnlyCollection<Keyword2> Keywords { get; }

        public Article(Guid id, IReadOnlyCollection<Keyword2> keywords)
        {
            Id = id;
            Keywords = keywords ?? throw new ArgumentNullException(nameof(keywords));
        }

        public override string ToString()
        {
            return string.Join(" | ", Keywords.Select(x => x.ToString()).ToArray());
        }

        protected bool Equals(Article other)
        {
            return Id.Equals(other.Id) && Keywords.SequenceEqual(other.Keywords);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Article) obj);
        }

        public override int GetHashCode()
        {
            unchecked {
                var hash = 19;
                foreach (var keyword in Keywords) {
                    hash = hash * 31 + keyword.GetHashCode();
                }
                return hash;
            }
        }
    }
}