using System.Collections.Generic;
using System.Linq;

namespace Sociomedia.Articles.Domain
{
    public static class StringExtensions
    {
        public static string ConcatWords(this IEnumerable<string> words)
        {
            return string.Join(' ', words);
        }

        public static bool Is3LettersAcronym(this string word)
        {
            return word.Length == 3 && word.All(char.IsUpper);
        }

        public static bool IsANumber(this string word)
        {
            return word.All(char.IsDigit);
        }
    }
}