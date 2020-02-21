using FluentAssertions;
using Xunit;

namespace NewsAggregator.Tests
{
    public class HtmlParserTests
    {
        private readonly HtmlParser _htmlParser;

        public HtmlParserTests()
        {
            _htmlParser = new HtmlParser();
        }

        [Fact]
        public void Extract_article_tag_from_html()
        {
            const string html = @"
                <html>
                    <body>
                    <p>text outside article</p>
                    <article>
                        <p class=""article__paragraph "">
                            Simple article content
                        </p>
                    </article>
                    <script>alert('some script')</script>
                </html>";

            var htmlArticle = _htmlParser.FindArticle(html);

            htmlArticle
                .Should()
                .Be(@"<article>
                        <p class=""article__paragraph "">
                            Simple article content
                        </p>
                    </article>");
        }

        [Fact]
        public void Return_html_input_if_no_article_found()
        {
            const string html = @"
                <html>
                    <body>
                    <p>text outside article</p>
                    <script>alert('some script')</script>
                </html>";

            var htmlArticle = _htmlParser.FindArticle(html);

            htmlArticle
                .Should()
                .Be(html);
        }
    }
}