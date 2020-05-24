using FluentAssertions;
using NSubstitute;
using Sociomedia.Articles.Domain;
using Xunit;

namespace Sociomedia.Articles.Tests.UnitTests
{
    public class KeywordsParserTests
    {
        private readonly KeywordsParser _keywordsParser;
        private readonly IKeywordDictionary _keywordDictionary;

        public KeywordsParserTests()
        {
            _keywordDictionary = Substitute.For<IKeywordDictionary>();
            _keywordDictionary.IsValidKeyword(Arg.Any<string>()).Returns(true);

            _keywordsParser = new KeywordsParser(_keywordDictionary);
        }

        [Theory]
        [InlineData("test a ad ax test")]
        [InlineData("a bc c test a test")]
        public void Keywords_are_words_with_more_than_2_letters(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .BeEquivalentTo(new Keyword("test", 2));
        }

        [Theory]
        [InlineData("I love cats, yes love cats !")]
        public void A_keyword_is_must_be_validated_by_dictionary(string text)
        {
            _keywordDictionary.IsValidKeyword("love").Returns(false);

            _keywordsParser.Parse(text)
                .Should()
                .Contain(new Keyword("cats", 2));
        }

        [Fact]
        public void Keywords_are_ordered_by_occurrence()
        {
            const string text = "c winter d test test winter john a winter test john b test";

            var keywords = _keywordsParser.Parse(text);

            keywords
                .Should()
                .BeEquivalentTo(new[] {
                        new Keyword("test", 4),
                        new Keyword("winter", 3),
                        new Keyword("john", 2),
                    }
                );
        }

        [Theory]
        [InlineData("john wick winter john wick summer")]
        [InlineData("john wick is a real john wick")]
        public void Keywords_can_be_a_combination_of_two_words(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .Contain(new Keyword("john wick", 2));
        }

        [Theory]
        [InlineData("I said to you, john wick rocks ! Yes, john wick rocks.")]
        [InlineData("john wick rocks winter john wick rocks")]
        public void Keywords_can_be_a_combination_of_three_words(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .Contain(new Keyword("john wick rocks", 2));
        }

        [Theory]
        [InlineData("TEST a Test b test c tést")]
        public void Keyword_is_a_group_of_words_ignoring_case_or_diacritics(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .BeEquivalentTo(new Keyword("test", 4));
        }

        [Theory]
        [InlineData("être ou ne pas être")]
        public void Keyword_can_have_diacritics(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .BeEquivalentTo(new Keyword("être", 2));
        }

        [Theory]
        [InlineData("John wick enjoys killing during winter time, and John wick is great.")]
        public void A_keyword_with_multiple_words_do_not_override_keyword_with_single(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .Contain(new Keyword("john wick", 2))
                .And.Contain(new Keyword("john", 1))
                .And.Contain(new Keyword("wick", 1));
        }

        [Theory]
        [InlineData("2020 a 2020 a 9001 a 9001")]
        public void A_keyword_can_not_contain_only_digits(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .BeEmpty();
        }

        [Theory]
        [InlineData("COVID-19 b COVID-19")]
        public void A_keyword_can_contain_some_digit(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .Contain(new Keyword("covid-19", 2));
        }

        [Theory]
        [InlineData("Le président est Jean Petit. Jean Petit aime diriger.")]
        public void A_keyword_can_be_a_first_name_and_last_name(string text)
        {
            _keywordDictionary.IsValidKeyword("Petit").Returns(false);
            
            _keywordsParser.Parse(text)
                .Should()
                .Contain(new Keyword("jean petit", 2));
        }

        [Theory]
        [InlineData("John wick enjoys killing during winter time, and John wick is great. Winter is great. Winter is cold.")]
        public void Order_keywords_by_occurence_and_then_word_count_descending(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .ContainInOrder(new [] {
                    new Keyword("winter", 3),
                    new Keyword("john wick", 2),
                    new Keyword("john", 2),
                    new Keyword("wick", 2), 
                });
        }
    }
}