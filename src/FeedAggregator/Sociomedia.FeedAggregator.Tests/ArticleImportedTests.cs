using System;
using FluentAssertions;
using Sociomedia.DomainEvents.Article;
using Xunit;

namespace Sociomedia.Tests
{
    public class ArticleImportedTests
    {
        [Fact]
        public void Event_stream_is_composed_with_category_and_event_id()
        {
            var id = Guid.NewGuid();

            var @event = new ArticleImported(id, null, null, default, null, null, null, default);

            @event.EventStream.Should().Be("article-" + id);
        }
    }
}