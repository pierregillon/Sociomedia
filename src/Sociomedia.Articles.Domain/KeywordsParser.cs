using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Domain;

namespace Sociomedia.Articles.Domain
{
    public class KeywordsParser
    {
        private readonly IKeywordDictionary _keywordDictionary;

        private const int COMPOSED_WORD_MAX_SIZE = 4;
        private const int WORD_MIN_OCCURENCE = 2;
        private const int MIN_WORD_LENGTH = 2;

        private static readonly string[] Separators = { " ", "\"", "'", "’", "«", "»", "?", "!", ";", ",", "." };

        public KeywordsParser(IKeywordDictionary keywordDictionary)
        {
            _keywordDictionary = keywordDictionary;
        }

        public IEnumerable<Keyword> Parse(string text)
        {
            return text
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(ReplaceWordIfInvalid)
                .ToArray()
                .Pipe(TransformToKeywords)
                .Pipe(FilterOnlyNewKeywords)
                .OrderByDescending(x => x.Occurence)
                .ThenBy(x => x.WordCount);
        }

        private string ReplaceWordIfInvalid(string word)
        {
            return word.Length > MIN_WORD_LENGTH && _keywordDictionary.IsNoun(word) ? word : null;
        }

        private IEnumerable<Keyword> TransformToKeywords(IReadOnlyCollection<string> words)
        {
            if (words.Count == 0) {
                yield break;
            }

            for (var combinationSize = COMPOSED_WORD_MAX_SIZE; combinationSize >= 1; combinationSize--) {
                var keywords = GetKeywordsComposed(words, combinationSize);
                foreach (var keyword in keywords) {
                    yield return keyword;
                }
            }
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

        public IEnumerable<Keyword> GetKeywordsComposed(IReadOnlyCollection<string> words, int combinationSize)
        {
            return Enumerable.Range(0, combinationSize)
                .SelectMany(x => words.Skip(x).Chunk(combinationSize))
                .Where(x => x.All(w => w != null))
                .GroupBy(x => string.Join(' ', x).RemoveDiacritics().ToLower())
                .Where(x => x.Count() >= WORD_MIN_OCCURENCE)
                .Select(x => new Keyword(x.Key, x.Count()));
        }
    }
}