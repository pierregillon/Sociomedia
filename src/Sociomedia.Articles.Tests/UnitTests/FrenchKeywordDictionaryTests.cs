using FluentAssertions;
using Sociomedia.Articles.Infrastructure;
using Xunit;

namespace Sociomedia.Articles.Tests.UnitTests
{
    public class FrenchKeywordDictionaryTests
    {
        private static readonly FrenchKeywordDictionary Dictionary;

        static FrenchKeywordDictionaryTests()
        {
            Dictionary = new FrenchKeywordDictionary("./Dictionaries/french.csv", "./Dictionaries/french_black_list.txt");
            Dictionary.BuildFromFiles();
        }

        [Theory]
        [InlineData("aventure")]
        [InlineData("chat")]
        [InlineData("écologiste")]
        [InlineData("climatique")]
        [InlineData("écologique")]
        public void Valid_keyword_is_a_noun_or_an_adjective(string noun)
        {
            Dictionary.IsValidKeyword(noun).Should().BeTrue();
        }

        [Theory]
        [InlineData("mais")]
        [InlineData("selon")]
        [InlineData("être")]
        [InlineData("promo")]
        [InlineData("promotion")]
        [InlineData("partage")]
        [InlineData("partager")]
        [InlineData("lire")]
        [InlineData("connexion")]
        [InlineData("plus")]
        [InlineData("pour")]
        [InlineData("fait")]
        [InlineData("moins")]
        [InlineData("est")]
        public void Some_words_are_forbidden(string adjective)
        {
            Dictionary.IsValidKeyword(adjective).Should().BeFalse();
        }

        [Theory]
        [InlineData("trump")]
        [InlineData("coronavirus")]
        [InlineData("rachida dati")]
        [InlineData("l214")]
        [InlineData("OMS")]
        public void Valid_keyword_is_unknown_word(string word)
        {
            Dictionary.IsValidKeyword(word).Should().BeTrue();
        }

        [Theory]
        [InlineData("le")]
        [InlineData("de")]
        [InlineData("mais")]
        [InlineData("depuis")]
        public void Invalid_keyword_is_article_or_auxiliary(string word)
        {
            Dictionary.IsValidKeyword(word).Should().BeFalse();
        }
    }
}