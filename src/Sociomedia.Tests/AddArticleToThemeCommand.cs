using System;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Application;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Tests {
    public class AddArticleToThemeCommand : ICommand
    {
        public Guid ThemeId { get; }
        public Article Article { get; }

        public AddArticleToThemeCommand(Guid themeId, Article article)
        {
            ThemeId = themeId;
            Article = article;
        }
    }
}