using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator
{
    public class KeywordsParser
    {
        private const int MaxCombinationWordSize = 4;
        private static readonly string[] Separators = { " ", "\"", "'", "’", "«", "»", "?", "!", ";", ",", "." };
        private static readonly string[] InvalidWords = { "mais", "donc", "dans", "aussi", "alors", "ensuite", "pour" };

        public IEnumerable<Keyword> Parse(string text)
        {
            var words = text
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Length <= 3 || InvalidWords.Contains(x) ? null : x)
                .ToArray();

            return FilterOnlyNewKeywords(GetKeywords(words))
                .OrderByDescending(x => x.Occurence)
                .ThenBy(x => x.WordCount);
        }

        private static IEnumerable<Keyword> FilterOnlyNewKeywords(IEnumerable<Keyword> keywords)
        {
            var allKeywords = new List<Keyword>();
            foreach (var keyword in keywords) {
                var existing = allKeywords.FirstOrDefault(x => x.Contains(keyword));
                if (existing != null && existing.Occurence == keyword.Occurence) {
                    continue;
                }
                allKeywords.Add(keyword);
                yield return keyword;
            }
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
                    yield return keyword;
                }

                if (wordProcessing.Count == 0) {
                    break;
                }
            }
        }

        public IEnumerable<Keyword> GetKeywordsComposed(IReadOnlyCollection<string> words, int combinationSize)
        {
            return Enumerable.Range(0, combinationSize)
                .SelectMany(x => words.Skip(x).Chunk(combinationSize))
                .Where(x => x.All(w => w != null))
                .GroupBy(x => string.Join(' ', x).RemoveDiacritics().ToLower())
                .Where(x => x.Count() >= 2)
                .Select(x => new Keyword(x.Key, x.Count()));
        }
    }
}