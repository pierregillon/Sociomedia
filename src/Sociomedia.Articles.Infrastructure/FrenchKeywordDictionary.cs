using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sociomedia.Articles.Domain;

namespace Sociomedia.Articles.Infrastructure
{
    public class FrenchKeywordDictionary : IKeywordDictionary
    {
        private readonly Dictionary<char, string[]> _nouns;

        public FrenchKeywordDictionary(string fileName)
        {
            var content = File.ReadAllText(fileName);

            _nouns = content
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(x => x[0])
                .ToDictionary(x => x.Key, x => x.ToArray());
        }

        public bool IsValidKeyword(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(word));
            var lowerCase = word.ToLowerInvariant();
            return _nouns.TryGetValue(lowerCase[0], out var list) && list.Contains(lowerCase);
        }
    }
}