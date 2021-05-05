using System;

namespace Sociomedia.Themes.Domain
{
    public class ArticleAddedToTheme : ThemeEvent
    {
        public Guid ArticleId { get; }

        public ArticleAddedToTheme(Guid themeId, Guid articleId) : base(themeId)
        {
            ArticleId = articleId;
        }
    }
}