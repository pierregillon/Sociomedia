using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator
{
    public class Keyword
    {
        private readonly string _value;
        private readonly IReadOnlyCollection<string> _words;

        public int Occurence { get; }
        public int WordCount => _words.Count;

        public Keyword(string value, int occurence)
        {
            _value = value;
            _words = value.Split(' ');
            Occurence = occurence;
        }

        public bool Contains(Keyword keyword)
        {
            return _words.Except(keyword._words).Count() != _words.Count;
        }

        public override string ToString()
        {
            return _value;
        }

        protected bool Equals(Keyword other)
        {
            return _words.SequenceEqual(other._words);
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
            return (_words != null ? _words.GetHashCode() : 0);
        }
    }
}