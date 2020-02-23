﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NewsAggregator
{
    public class KeywordsParser
    {
        private const int MaxCombinationWordSize = 4;
        private static readonly string[] Separators = { " ", "\"", "'", "«", "»", "?", "!", ";", ",", "." };
        private static readonly string[] InvalidWords = { "mais", "donc", "dans", "aussi", "alors", "ensuite", "pour" };

        public IReadOnlyCollection<Keyword> Parse(string text, int count)
        {
            var words = text
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Length <= 3 || InvalidWords.Contains(x) ? null : x)
                .ToArray();

            return FilterOnlyNewKeywords(GetKeywords(words))
                .OrderByDescending(x => x.Occurence)
                .Take(count)
                .ToArray();
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
            var matrix = Enumerable.Range(0, combinationSize)
                .SelectMany(x => words.Skip(x).Chunk(combinationSize))
                .Where(x => x.All(w => w != null))
                .Select(x => string.Join(' ', x));

            return matrix
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
        public int WordCount => Value.Split(' ').Length;

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