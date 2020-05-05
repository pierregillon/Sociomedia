using System;

namespace Sociomedia.FeedAggregator.Domain.Themes
{
    public class ArticleAddedToTheme : DomainEvent
    {
        public ArticleAddedToTheme(Guid themeId, ThemeArticle article)
        {
            Id = themeId;
            Article = article;
        }

        public ThemeArticle Article { get; }
    }
}