﻿using System;

namespace NewsAggregator.Domain.Rss
{
    public class ExternalArticle
    {
        public Uri Url { get; set; }
        public string Title { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Summary { get; set; }
        public Uri ImageUrl { get; set; }
    }
}