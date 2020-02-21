using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator
{
    public class KeywordsCalculator
    {
        public IReadOnlyCollection<Keyword> Calculate(IReadOnlyCollection<string> words, int count)
        {
            return words
                .Where(x=>x.Length > 3)
                .GroupBy(x => x.ToLower())
                .OrderByDescending(x => x.Count())
                .Take(count)
                .Select(x => new Keyword(x.Key, x.Count()))
                .ToArray();
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
    }
}