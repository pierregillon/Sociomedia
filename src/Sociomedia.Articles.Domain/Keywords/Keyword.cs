using System;
using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.Articles.Domain.Keywords
{
    public class Keyword : IEquatable<Keyword>
    {
        private readonly IReadOnlyCollection<string> _words;

        public int Occurence { get; }
        public int WordCount => _words.Count;
        public string Value { get; }

        public Keyword(string value, int occurence)
        {
            Value = value;
            _words = value.Split(' ');
            Occurence = occurence;
        }

        public bool Contains(Keyword keyword)
        {
            return Value.Contains(keyword.Value);
        }

        public override string ToString()
        {
            return Value + $" ({Occurence})";
        }

        public bool Equals(Keyword other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return _words.SequenceEqual(other._words) && Occurence == other.Occurence;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Keyword) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static Keyword operator +(Keyword x, Keyword y)
        {
            if (x.Value != y.Value) {
                throw new InvalidOperationException("Keywords are different, unable to add them");
            }
            return new Keyword(x.Value, x.Occurence + y.Occurence);
        }
    }
}