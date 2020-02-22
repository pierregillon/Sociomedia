using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator
{
    public class KeywordsCalculator
    {
        private const int MaxCombinationWordSize = 4;

        public IReadOnlyCollection<Keyword> Calculate(IReadOnlyCollection<string> words, int count)
        {
            return GetKeywords(words)
                .Where(x => x.Value.Length > 3)
                .Take(count)
                .ToArray();
        }

        private IEnumerable<Keyword> GetKeywords(IEnumerable<string> words)
        {
            var wordProcessing = words.ToList();

            for (var combinationSize = MaxCombinationWordSize; combinationSize >= 1; combinationSize--) {
                if (wordProcessing.Count <= combinationSize) {
                    continue;
                }

                var keywords = GetKeywordsComposed(wordProcessing, combinationSize);
                foreach (var keyword in keywords) {
                    yield return keyword;
                }

                wordProcessing.RemoveAll(x => keywords.Any(keyword => keyword.Contains(x)));

                if (wordProcessing.Count == 0) {
                    break;
                }
            }
        }

        public IReadOnlyCollection<Keyword> GetKeywordsComposed(IReadOnlyCollection<string> words, int combinationSize)
        {
            var keywordsGroups = new List<IEnumerable<string>>();

            for (var i = 0; i < combinationSize / 2 + 1; i++) {
                keywordsGroups.AddRange(words.Skip(i).Chunk(combinationSize).ToArray());
            }

            return keywordsGroups
                .Select(x => string.Join(' ', x))
                .GroupBy(x => x)
                .Where(x => combinationSize == 1 || x.Count() >= 2)
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

        public bool Contains(string word)
        {
            return Value.Contains(word);
        }
    }
}