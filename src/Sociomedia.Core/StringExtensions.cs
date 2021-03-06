﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sociomedia.Core
{
    public static class StringExtensions
    {
        public static string SeparatePascalCaseWords(this string input)
        {
            static IEnumerable<int> GetIndexes(string name)
            {
                for (var i = 1; i < name.Length; i++) {
                    if (!char.IsUpper(name[i - 1]) && char.IsUpper(name[i])) {
                        yield return i;
                    }
                    else if (i < name.Length - 1 && char.IsUpper(name[i - 1]) && char.IsUpper(name[i]) && !char.IsUpper(name[i + 1])) {
                        yield return i;
                    }
                }
            }

            foreach (var index in GetIndexes(input).Reverse()) {
                input = input.Insert(index, "_");
            }
            return input;
        }

        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
 }