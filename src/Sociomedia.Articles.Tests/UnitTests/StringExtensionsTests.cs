using FluentAssertions;
using Sociomedia.Articles.Domain;
using Sociomedia.Core;
using Xunit;

namespace Sociomedia.Articles.Tests.UnitTests
{
    public class StringExtensionsTests
    {
        public class SeparatePascalCaseWords
        {
            [Fact]
            public void Add_underscore_between_lower_case_letter_following_by_upper_case_letter()
            {
                "ArticleProjectionSynchronizer"
                    .SeparatePascalCaseWords()
                    .Should()
                    .Be("Article_Projection_Synchronizer");
            }

            [Theory]
            [InlineData("HELLOWORLD")]
            [InlineData("helloworld")]
            public void Do_not_add_any_underscore_when_single_case(string value)
            {
                value
                    .SeparatePascalCaseWords()
                    .Should()
                    .Be(value);
            }

            [Fact]
            public void Add_extra_underscore_when_breaking_upper_case_consecutive_letters()
            {
                "SimpleTESTWithConsecutiveUpperCaseLetters"
                    .SeparatePascalCaseWords()
                    .Should()
                    .Be("Simple_TEST_With_Consecutive_Upper_Case_Letters");
            }
        }

        public class ConcatWords
        {
            [Fact]
            public void Concat_different_words_with_space()
            {
                new[] { "hello", "world", "how", "are", "you", "?" }
                    .ConcatWords()
                    .Should()
                    .Be("hello world how are you ?");
            }
        }
    }
}