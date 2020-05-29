using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Core.Application;
using Sociomedia.Themes.Application.Commands.AddArticleToTheme;
using Sociomedia.Themes.Application.Commands.CreateNewTheme;
using Sociomedia.Themes.Domain;

namespace Sociomedia.Themes.Application
{
    public class ThemeManager2
    {
        private readonly ThemeProjection _themeProjection;
        private readonly List<AddArticleToThemeCommand> _commands = new List<AddArticleToThemeCommand>();


        public ThemeManager2(ThemeProjection themeProjection)
        {
            _themeProjection = themeProjection;
        }

        public IEnumerable<ICommand> Add(Article article)
        {
            foreach (var existingArticle in _themeProjection.Articles) {
                var keywordIntersection = existingArticle.CommonKeywords(article);
                if (!keywordIntersection.Any()) {
                    continue;
                }
                var matchingThemes = _themeProjection.Themes.Where(theme => keywordIntersection.ContainsAllWords(theme.Keywords)).ToList();
                if (matchingThemes.Any()) {
                    foreach (var matchingTheme in matchingThemes.Where(matchingTheme => !matchingTheme.Contains(article))) {
                        if (AddArticleToTheme(matchingTheme.Id, article, out var command)) {
                            yield return command;
                        }
                    }
                }
                if (!matchingThemes.Any() || matchingThemes.All(x => x.Keywords.Count != keywordIntersection.Count)) {
                    yield return new CreateNewThemeCommand(keywordIntersection.ToArray(), new[] { article.Id, existingArticle.Id });
                }
            }
        }

        private bool AddArticleToTheme(Guid themeId, Article article, out AddArticleToThemeCommand command)
        {
            command = new AddArticleToThemeCommand(themeId, article);
            if (!_commands.Any(x => x.Article.Id == article.Id && x.ThemeId == themeId)) {
                _commands.Add(command);
                return true;
            }
            return false;
        }
    }
}