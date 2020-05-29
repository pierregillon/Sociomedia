using System;
using System.Collections.Generic;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application.Commands.CreateNewTheme
{
    public class CreateNewThemeCommand : ICommand
    {
        public IReadOnlyCollection<Keyword2> Keywords { get; }
        public IReadOnlyCollection<Guid> Articles { get; }

        public CreateNewThemeCommand(IReadOnlyCollection<Keyword2> keywords, IReadOnlyCollection<Guid> articles)
        {
            Keywords = keywords;
            Articles = articles;
        }
    }
}