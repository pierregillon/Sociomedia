using System;

namespace Sociomedia.Themes.Domain
{
    public class Keyword : IEquatable<Keyword>
    {
        public int Occurence { get; }
        public string Value { get; }

        public Keyword(string value, int occurence)
        {
            Value = value;
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
            return Equals(Value, other.Value) && Equals(Occurence, other.Occurence);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Keyword) obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static Keyword operator +(Keyword x, Keyword y)
        {
            if (x.Value != y.Value) {
                throw new InvalidOperationException("Keywords are different, unable to add them");
            }
            return new Keyword(x.Value, x.Occurence + y.Occurence);
        }
    }
}