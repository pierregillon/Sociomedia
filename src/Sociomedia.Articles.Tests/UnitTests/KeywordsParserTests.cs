﻿using FluentAssertions;
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
        [InlineData("I love cat, yes love cat !")]
        public void A_keyword_is_must_be_validated_by_dictionary(string text)
        {
            _keywordDictionary.IsValidKeyword("love").Returns(false);

            _keywordsParser.Parse(text)
                .Should()
                .Contain(new Keyword("cat", 1));
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
        [InlineData("John wick enjoys killing during winter time. Of course, john wick enjoys killing in summer time too.")]
        public void Keywords_can_be_a_combination_four_words(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .Contain(new Keyword("john wick enjoys killing", 2));
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
        public void A_keyword_with_multiple_words_overrides_other_keyword(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .Contain(new Keyword("john wick", 2))
                .And.NotContain(new Keyword("john", 1))
                .And.NotContain(new Keyword("wick", 1));
        }

        [Theory]
        [InlineData("2020 a 2020 a test2 a test2")]
        public void A_keyword_does_not_contain_digit(string text)
        {
            _keywordsParser.Parse(text)
                .Should()
                .BeEmpty();
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
    }
}