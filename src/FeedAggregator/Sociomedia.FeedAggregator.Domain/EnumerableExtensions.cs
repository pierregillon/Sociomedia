using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.FeedAggregator.Domain
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> values, int chunkSize)
        {
            using var enumerator = values.GetEnumerator();
            while (enumerator.MoveNext()) {
                yield return GetChunk(enumerator, chunkSize).ToList();
            }
        }

        private static IEnumerable<T> GetChunk<T>(IEnumerator<T> enumerator, int chunkSize)
        {
            do {
                yield return enumerator.Current;
            } while (--chunkSize > 0 && enumerator.MoveNext());
        }

        public static IEnumerable<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> values)
        {
            IEnumerable<T> result = null;
            using var enumerator = values.GetEnumerator();
            while (enumerator.MoveNext()) {
                result = result == null ? enumerator.Current : result.Intersect(enumerator.Current);
            }
            return result;
        }
    }
}