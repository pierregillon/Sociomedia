using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NewsAggregator
{
    public class KeywordsCalculator
    {
        private const int MaxCombinationWordSize = 4;

        public IReadOnlyCollection<Keyword> Calculate(IReadOnlyCollection<string> words, int count)
        {
            return GetKeywords(words)
                .Take(count)
                .ToArray();
        }

        private IEnumerable<Keyword> GetKeywords(IEnumerable<string> words)
        {
            var wordProcessing = words.ToList();

            for (var combinationSize = MaxCombinationWordSize; combinationSize >= 1; combinationSize--) {
                if (wordProcessing.Count < combinationSize) {
                    continue;
                }

                var keywords = GetKeywordsComposed(wordProcessing, combinationSize);
                foreach (var keyword in keywords) {
                    wordProcessing.RemoveAll(x => keyword.Contains(x));
                    yield return keyword;
                }

                if (wordProcessing.Count == 0) {
                    break;
                }
            }
        }

        public IEnumerable<Keyword> GetKeywordsComposed(IReadOnlyCollection<string> words, int combinationSize)
        {
            return Enumerable.Range(0, combinationSize / 2 + 1)
                .SelectMany(x => words.Skip(x).Chunk(combinationSize))
                .Where(x => x.All(s => s.Length > 2))
                .Select(x => string.Join(' ', x))
                .GroupBy(x => x.RemoveDiacritics().ToLower())
                .Where(x => x.Count() >= 2)
                .OrderByDescending(x => x.Count())
                .Select(x => new Keyword(x.Key, x.Count()));
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

    public static class StringExtensions
    {
        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString) {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
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