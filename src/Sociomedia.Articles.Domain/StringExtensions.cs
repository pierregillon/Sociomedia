using System.Collections.Generic;

namespace Sociomedia.Articles.Domain
{
    public static class StringExtensions
    {
        public static string ConcatWords(this IEnumerable<string> words)
        {
            return string.Join(' ', words);
        }
    }
}