using FluentAssertions;
using Sociomedia.Articles.Infrastructure;
using Xunit;

namespace Sociomedia.Articles.Tests.UnitTests
{
    public class FrenchKeywordDictionaryTests
    {
        private readonly FrenchKeywordDictionary _dictionary;

        public FrenchKeywordDictionaryTests()
        {
            _dictionary = new FrenchKeywordDictionary("Dictionaries\\french_nouns.txt");
        }

        [Theory]
        [InlineData("voiture")]
        [InlineData("alimentation")]
        [InlineData("santé")]
        [InlineData("ordinateur")]
        [InlineData("information")]
        public void Is_a_french_noun(string word)
        {
            _dictionary.IsValidKeyword(word).Should().BeTrue();
        }


        [Theory]
        [InlineData("Voiture")]
        [InlineData("VOITURE")]
        public void Compare_in_case_invariant(string word)
        {
            _dictionary.IsValidKeyword(word).Should().BeTrue();
        }

        [Theory]
        [InlineData("tester")]
        [InlineData("assouplir")]
        [InlineData("réduire")]
        [InlineData("tant")]
        [InlineData("mais")]
        [InlineData("le")]
        [InlineData("du")]
        [InlineData("aventurais")]
        public void Is_not_a_french_noun(string word)
        {
            _dictionary.IsValidKeyword(word).Should().BeFalse();
        }
    }
}