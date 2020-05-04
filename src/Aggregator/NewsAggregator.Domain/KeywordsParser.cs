using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator.Domain
{
    public class KeywordsParser
    {
        private const int MaxCombinationWordSize = 4;
        private static readonly string[] Separators = { " ", "\"", "'", "’", "«", "»", "?", "!", ";", ",", "." };
        private static readonly string[] InvalidWords = { "mais", "donc", "dans", "aussi", "alors", "ensuite", "pour" };

        public IEnumerable<Keyword> Parse(string text)
        {
            return text
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(ReplaceWordIfInvalid)
                .ToArray()
                .Pipe(this.TransformToKeywords)
                .Pipe(FilterOnlyNewKeywords)
                .OrderByDescending(x => x.Occurence)
                .ThenBy(x => x.WordCount);
        }

        private static string ReplaceWordIfInvalid(string x)
        {
            return x.Length <= 3 || InvalidWords.Contains(x) ? null : x;
        }

        private IEnumerable<Keyword> TransformToKeywords(IReadOnlyCollection<string> words)
        {
            if (words.Count == 0) {
                yield break;
            }

            for (int combinationSize = MaxCombinationWordSize; combinationSize >= 1; combinationSize--) {
                IEnumerable<Keyword> keywords = this.GetKeywordsComposed(words, combinationSize);
                foreach (Keyword keyword in keywords) {
                    yield return keyword;
                }
            }
        }

        private static IEnumerable<Keyword> FilterOnlyNewKeywords(IEnumerable<Keyword> keywords)
        {
            List<Keyword> allKeywords = new List<Keyword>();
            foreach (Keyword keyword in keywords) {
                Keyword existing = allKeywords.FirstOrDefault(x => x.Contains(keyword));
                if (existing != null && existing.Occurence == keyword.Occurence) {
                    continue;
                }
                allKeywords.Add(keyword);
                yield return keyword;
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