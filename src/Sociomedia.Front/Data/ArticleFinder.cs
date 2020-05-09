using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Tools;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.Front.Data
{
    public class ArticleFinder
    {
        private readonly DbConnectionReadModel _dbConnection;

        public ArticleFinder(DbConnectionReadModel dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IReadOnlyCollection<ArticleListItem>> List(int page, int pageSize, Guid? mediaId = null, string keyword = default)
        {
            var query = BuildArticlesQuery(mediaId, keyword);

            if (page > 0) {
                query = query.Skip(page * pageSize);
            }

            query = query.Take(pageSize);

            return await query.ToArrayAsync();
        }

        public async Task<int> Count(Guid? mediaId, string keyword)
        {
            return await BuildArticlesQuery(mediaId, keyword).CountAsync();
        }

        private IQueryable<ArticleListItem> BuildArticlesQuery(Guid? mediaId = null, string keyword = default)
        {
            var query =
                from article in _dbConnection.Articles
                join media in _dbConnection.Medias on article.MediaId equals media.Id
                orderby article.PublishDate descending
                select new ArticleListItem {
                    Id = article.Id,
                    Title = article.Title,
                    Url = article.Url,
                    Summary = article.Summary,
                    ImageUrl = article.ImageUrl,
                    PublishDate = article.PublishDate,
                    MediaId = article.MediaId,
                    MediaImageUrl = media.ImageUrl
                };

            if (!string.IsNullOrWhiteSpace(keyword)) {
                keyword = RemoveDiacritics(keyword);

                var keywords = keyword
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.ToLower())
                    .ToArray();

                var articleContainingKeywords =
                    from key in _dbConnection.Keywords
                    where key.Value.In(keywords)
                    select key.FK_Article;

                query = query.Where(x => x.Id.In(articleContainingKeywords));
            }

            if (mediaId.HasValue) {
                query = query.Where(x => x.MediaId == mediaId.Value);
            }

            return query;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString) {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}