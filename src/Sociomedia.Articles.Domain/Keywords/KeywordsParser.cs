using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Domain;

namespace Sociomedia.Articles.Domain.Keywords
{
    public class KeywordsParser
    {
        private readonly IKeywordDictionary _keywordDictionary;

        private const int MAX_COMPOSED_WORD_LENGTH = 3;
        private const int MIN_WORD_OCCURENCE = 2;
        private const int MIN_WORD_LENGTH = 4;

        private static readonly string[] Separators = { " ", "\"", "'", "’", "“", "”", "«", "»", "?", "!", ";", ",", ".", ":", "(", ")" };

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
                .OrderByDescending(x => x.Occurence)
                .ThenByDescending(x => x.WordCount);
        }

        private string ReplaceWordIfInvalid(string word)
        {
            return IsWordEligible(word) ? word : null;
        }

        private bool IsWordEligible(string word)
        {
            return !string.IsNullOrWhiteSpace(word) 
                   && (word.Is3LettersAcronym() ||  word.Length >= MIN_WORD_LENGTH)
                   && !word.IsANumber()
                   && _keywordDictionary.IsValidKeyword(word);
        }

        private static IEnumerable<Keyword> TransformToKeywords(IReadOnlyCollection<string> words)
        {
            if (words.Count == 0) {
                yield break;
            }

            for (var combinationSize = MAX_COMPOSED_WORD_LENGTH; combinationSize >= 1; combinationSize--) {
                var keywords = GetKeywordsComposed(words, combinationSize);
                foreach (var keyword in keywords) {
                    yield return keyword;
                }
            }
        }

        private static IEnumerable<Keyword> GetKeywordsComposed(IReadOnlyCollection<string> words, int combinationSize)
        {
            return Enumerable.Range(0, combinationSize)
                .SelectMany(x => words.Skip(x).Chunk(combinationSize))
                .Where(x => x.All(w => w != null))
                .GroupBy(x => x.ConcatWords().RemoveDiacritics().ToLower())
                .Where(x => x.Count() >= MIN_WORD_OCCURENCE)
                .Select(x => new Keyword(x.First().ConcatWords().ToLower(), x.Count()));
        }
    }
}