using FluentAssertions;
using Sociomedia.Articles.Domain;
using Xunit;

namespace Sociomedia.Articles.Tests.UnitTests
{
    public class UrlSanitizerTests
    {
        [Fact]
        public void Clear_dimension_parameters()
        {
            var url = "https://medias.liberation.fr/photo/1312130-000_rj4a0.jpg?modified_at=1588944926&ratio_x=03&ratio_y=02&width=150";

            var sanitizedUrl = UrlSanitizer.Sanitize(url);

            sanitizedUrl
                .Should()
                .Be("https://medias.liberation.fr/photo/1312130-000_rj4a0.jpg");
        }

        [Fact]
        public void Keep_non_dimension_parameters()
        {
            var url = "https://medias.liberation.fr/photo/1311428-emmanuel-macron-dans-une-ecole-des-yvelines-le-5-mai-2020-a-poissy.jpg?modified_at=1588686002&ratio_x=03&ratio_y=02&width=150&test=1";

            var sanitizedUrl = UrlSanitizer.Sanitize(url);

            sanitizedUrl
                .Should()
                .Be("https://medias.liberation.fr/photo/1311428-emmanuel-macron-dans-une-ecole-des-yvelines-le-5-mai-2020-a-poissy.jpg?test=1");
        }
    }
}