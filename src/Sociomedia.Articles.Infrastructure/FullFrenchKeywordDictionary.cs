using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using Sociomedia.Articles.Domain;

namespace Sociomedia.Articles.Infrastructure
{
    public class FullFrenchKeywordDictionary : IKeywordDictionary
    {
        private readonly string _fileName;
        private readonly IDictionary<string, List<WordDescriptor>> _wordDescriptors = new Dictionary<string, List<WordDescriptor>>();
        private readonly IReadOnlyCollection<string> _specialForbiddenWords = new[] { "plus", "pour", "fait", "moins" };
        private bool _isDictionaryBuilt;

        public FullFrenchKeywordDictionary(string fileName)
        {
            _fileName = fileName;
        }

        public void BuildFromFile()
        {
            using var parser = new TextFieldParser(Path.Combine(_fileName), Encoding.UTF8) { TextFieldType = FieldType.Delimited };

            parser.SetDelimiters(";");

            while (!parser.EndOfData) {
                var fields = parser.ReadFields();
                AddWordDescriptor(fields[0], fields[3]);
            }

            _isDictionaryBuilt = true;
        }

        public bool IsValidKeyword(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(word));
            if (!_isDictionaryBuilt) {
                throw new InvalidOperationException("The french dictionary must be initialized first.");
            }

            var lowerCase = word.ToLower();
            if (_specialForbiddenWords.Contains(lowerCase)) {
                return false;
            }
            var key = GetKey(lowerCase);
            if (_wordDescriptors.TryGetValue(key, out var wordDescriptors)) {
                var wordDescriptor = wordDescriptors.FirstOrDefault(x => x.Word == lowerCase);
                return wordDescriptor == null || wordDescriptor.IsKeyWord;
            }
            return true;
        }

        private void AddWordDescriptor(string word, string type)
        {
            var key = GetKey(word);

            if (!_wordDescriptors.TryGetValue(key, out var wordDescriptors)) {
                _wordDescriptors.Add(key, new List<WordDescriptor> { new WordDescriptor(word, type) });
            }
            else {
                var wordDescriptor = wordDescriptors.FirstOrDefault(x => x.Word == word);
                if (wordDescriptor != null) {
                    wordDescriptor.AddType(type);
                }
                else {
                    wordDescriptors.Add(new WordDescriptor(word, type));
                }
            }
        }

        private static string GetKey(string word)
        {
            return word.Substring(0, Math.Min(word.Length, 3));
        }

        private class WordDescriptor
        {
            private readonly HashSet<string> _types;

            public string Word { get; }
            public bool IsKeyWord => _types.Contains("NOM") || _types.Contains("ADJ");

            public WordDescriptor(string word, string type)
            {
                Word = word;
                _types = new HashSet<string> { type };
            }

            public void AddType(string type)
            {
                _types.Add(type);
            }
        }
    }
}