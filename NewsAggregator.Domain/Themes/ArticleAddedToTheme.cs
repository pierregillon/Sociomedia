using System;

namespace NewsAggregator.Domain.Themes {
    public class ArticleAddedToTheme : IDomainEvent
    {
        public Guid ThemeId { get; }
        public ThemeArticle Article { get; }

        public ArticleAddedToTheme(Guid themeId, ThemeArticle article)
        {
            ThemeId = themeId;
            Article = article;
        }
    }
}