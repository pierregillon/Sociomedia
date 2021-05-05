using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Domain;

namespace Sociomedia.Themes.Domain
{
    public class Keywords
    {
        private readonly IReadOnlyCollection<string> _values;

        public Keywords(IEnumerable<string> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            _values = values.OrderBy(x => x).ToArray();
        }

        public bool Any()
        {
            return _values.Any();
        }

        public bool IsCompatibleForThemeCreation()
        {
            return _values.Any() && _values.Count <= 5;
        }

        public bool ContainsAll(Keywords keywords)
        {
            return !keywords._values.Except(_values).Any();
        }

        public bool SequenceEqual(Keywords keywords)
        {
            return _values.Count == keywords._values.Count && _values.SequenceEqual(keywords._values);
        }

        public Keywords Intersect(Keywords keywords)
        {
            return _values
                .Intersect(keywords._values)
                .ToArray()
                .Pipe(x => new Keywords(x));
        }

        public IReadOnlyCollection<string> ToValues()
        {
            return _values;
        }

        public override string ToString()
        {
            return string.Join(" | ", _values);
        }

        public override int GetHashCode()
        {
            unchecked {
                var hash = 19;
                foreach (var keyword in _values) {
                    hash = hash * 31 + keyword.GetHashCode();
                }
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Keywords other) {
                return SequenceEqual(other);
            }
            return false;
        }
    }
}