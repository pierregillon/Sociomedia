using System;
using System.Collections.Generic;

namespace Sociomedia.ProjectionSynchronizer.Application.Events
{
    public class ArticleSynchronized : IDomainEvent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public IReadOnlyCollection<string> Keywords { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
    }
}