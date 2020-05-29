using System;
using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.Themes.Domain
{
    public class Keyword2 : IEquatable<Keyword2>
    {
        private readonly IReadOnlyCollection<string> _words;

        public int Occurence { get; }
        public int WordCount => _words.Count;
        public string Value { get; }

        public Keyword2(string value, int occurence)
        {
            Value = value;
            _words = value.Split(' ');
            Occurence = occurence;
        }

        public bool Contains(Keyword2 keyword2)
        {
            return Value.Contains(keyword2.Value);
        }

        public override string ToString()
        {
            return Value + $" ({Occurence})";
        }

        public bool Equals(Keyword2 other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return _words.SequenceEqual(other._words) && Occurence == other.Occurence;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Keyword2) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static Keyword2 operator +(Keyword2 x, Keyword2 y)
        {
            if (x.Value != y.Value) {
                throw new InvalidOperationException("Keywords are different, unable to add them");
            }
            return new Keyword2(x.Value, x.Occurence + y.Occurence);
        }
    }
}