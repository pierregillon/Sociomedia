using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sociomedia.Themes.Application.Projections
{
    public class ThemeProjectionRepository
    {
        private readonly IDictionary<Guid, ThemeReadModel> _themesDictionary = new Dictionary<Guid, ThemeReadModel>();
        private readonly IDictionary<Guid, ArticleReadModel> _articlesDictionary = new Dictionary<Guid, ArticleReadModel>();
        private readonly List<ArticleReadModel> _articlesList = new List<ArticleReadModel>();
        private readonly List<ThemeReadModel> _themeList = new List<ThemeReadModel>();

        public Task AddArticle(ArticleReadModel article)
        {
            _articlesDictionary.Add(article.Id, article);
            _articlesList.Add(article);
            return Task.CompletedTask;
        }

        public Task AddTheme(ThemeReadModel theme)
        {
            _themesDictionary.Add(theme.Id, theme);
            _themeList.Add(theme);
            return Task.CompletedTask;
        }

        public ThemeReadModel FindTheme(Guid themeId)
        {
            return _themesDictionary.TryGetValue(themeId, out var theme)
                ? theme
                : null;
        }

        public ArticleReadModel FindArticle(Guid articleId)
        {
            return _articlesDictionary.TryGetValue(articleId, out var article)
                ? article
                : null;
        }

        public IReadOnlyCollection<ArticleReadModel> FindArticles(IEnumerable<Guid> articleIds)
        {
            return articleIds.Select(FindArticle).Where(x => x != null).ToArray();
        }

        public IReadOnlyCollection<ThemeReadModel> GetAllThemes()
        {
            return _themeList;
        }

        public IReadOnlyCollection<ArticleReadModel> GetAllArticles()
        {
            return _articlesList;
        }
    }
}