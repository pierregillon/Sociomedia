using System;
using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.ProjectionSynchronizer.Application
{
    public static class StringExtensions
    {
        public static string FirstLetterUpper(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) {
                return value;
            }
            if (char.IsUpper(value[0])) {
                return value;
            }
            return new string(value.Skip(1).Prepend(char.ToUpper(value[0])).ToArray());
        }

        public static string FirstLetterUpperForEachWords(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) {
                return value;
            }

            return value
                .GetWords()
                .Select(x => x.FirstLetterUpper())
                .Aggregate((x, y) => x + " " + y);
        }

        public static IEnumerable<string> GetWords(this string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence)) {
                return Enumerable.Empty<string>();
            }

            return sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}