﻿using System;

namespace NewsAggregator.Domain.Rss
{
    public class RssFeed
    {
        public DateTime LastUpdateDate { get; set; }
        public string Url { get; set; }
        public string Html { get; set; }
        public string Id { get; set; }
    }
}