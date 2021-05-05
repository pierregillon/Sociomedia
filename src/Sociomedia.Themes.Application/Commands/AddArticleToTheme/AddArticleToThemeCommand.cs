using System;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Commands.AddArticleToTheme
{
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