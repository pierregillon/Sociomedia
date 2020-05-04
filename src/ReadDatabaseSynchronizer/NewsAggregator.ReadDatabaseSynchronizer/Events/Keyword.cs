using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator.ReadDatabaseSynchronizer.Events
{
    public class Keyword : IEquatable<Keyword>
    {
        private readonly string _value;
        private readonly IReadOnlyCollection<string> _words;

        public int Occurence { get; }
        public int WordCount => _words.Count;
        public IEnumerable<string> Words => _words;

        public Keyword(string value, int occurence)
        {
            _value = value;
            _words = value.Split(' ');
            Occurence = occurence;
        }

        public bool Contains(Keyword keyword)
        {
            return _value.Contains(keyword._value);
        }

        public override string ToString()
        {
            return _value;
        }

        public bool Equals(Keyword other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return _words.SequenceEqual(other._words);
            //return string.Compare(_value, other._value, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Keyword) obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}