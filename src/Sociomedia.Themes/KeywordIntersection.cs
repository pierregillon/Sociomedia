using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.Themes.Domain
{
    public class KeywordIntersection
    {
        private readonly IReadOnlyCollection<string> _keywords;

        public KeywordIntersection(IReadOnlyCollection<string> keywords)
        {
            _keywords = keywords;
        }

        public int Count => _keywords.Count;

        public bool Any()
        {
            return _keywords.Any();
        }

        public bool ContainsAllWords(IReadOnlyCollection<string> keywords)
        {
            return !keywords.Except(_keywords).Any();
        }

        public bool SequenceEquals(IReadOnlyCollection<string> keywords)
        {
            return _keywords.SequenceEqual(keywords);
        }

        public override string ToString()
        {
            return string.Join(" | ", _keywords);
        }

        public override int GetHashCode()
        {
            unchecked {
                var hash = 19;
                foreach (var keyword in _keywords) {
                    hash = hash * 31 + keyword.GetHashCode();
                }
                return hash;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is KeywordIntersection other) {
                return other.SequenceEquals(_keywords);
            }
            return false;
        }
    }
}