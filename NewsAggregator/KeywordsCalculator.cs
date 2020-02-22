using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator
{
    public class KeywordsCalculator
    {
        public IReadOnlyCollection<Keyword> Calculate(IReadOnlyCollection<string> words, int count)
        {
            var results = new List<Keyword>();
            for (var i = 4; i >= 1; i--) {
                var keywords = GetKeywordsComposed(words, i);
                results.AddRange(keywords.Where(x=> !results.Any(y => y.Contains(x))));
            }

            return results
                .Where(x => x.Value.Length > 3)
                .Take(count)
                .ToArray();
        }

        public IReadOnlyCollection<Keyword> GetKeywordsComposed(IReadOnlyCollection<string> words, int count)
        {
            var test = new List<IEnumerable<string>>();

            for (int i = 0; i < count/2 + 1; i++) {
                test.AddRange(words.Skip(i).Chunk(count).ToArray());
            }

            return test
                .Select(x => string.Join(' ', x))
                .GroupBy(x => x)
                .Where(x => count == 1 || x.Count() >= 2)
                .OrderByDescending(x => x.Count())
                .Select(x => new Keyword(x.Key, x.Count()))
                .ToArray();
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            while (source.Any()) {
                yield return source.Take(chunkSize);
                source = source.Skip(chunkSize);
            }
        }
    }

    public class Keyword
    {
        public string Value { get; }
        public int Occurence { get; }

        public Keyword(string value, int occurence)
        {
            Value = value;
            Occurence = occurence;
        }

        public bool Contains(Keyword keyword)
        {
            var words = Value.Split(' ');
            return words.Except(keyword.Value.Split(' ')).Count() != words.Length;
        }
    }
}