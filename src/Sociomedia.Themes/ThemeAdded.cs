﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.Themes.Domain
{
    public class ThemeAdded : ThemeEvent
    {
        public IReadOnlyCollection<Keyword> Keywords { get; }
        public IReadOnlyCollection<Guid> Articles { get; }

        public ThemeAdded(Guid id, IReadOnlyCollection<Keyword> keywords, IReadOnlyCollection<Guid> articles) : base(id)
        {
            if (articles.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(articles));
            if (articles.Count != articles.Distinct().Count()) {
                throw new ArgumentException("Articles must be unique");
            }


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