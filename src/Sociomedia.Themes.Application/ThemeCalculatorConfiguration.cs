using System;

namespace Sociomedia.Themes.Application
{
    public class ThemeCalculatorConfiguration
    {
        public int ArticleAggregationIntervalInDays { get; set; }

        public TimeSpan ArticleAggregationInterval => TimeSpan.FromDays(ArticleAggregationIntervalInDays);
    }
}