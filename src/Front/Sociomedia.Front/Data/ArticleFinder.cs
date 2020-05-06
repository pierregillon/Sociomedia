using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
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

        public async Task<IReadOnlyCollection<ArticleListItem>> List()
        {
            return await _dbConnection.Articles
                .OrderByDescending(x => x.PublishDate)
                .Select(x => new ArticleListItem {
                    Id = x.Id,
                    Title = x.Title,
                    Url = x.Url,
                    Summary = x.Summary,
                    ImageUrl = x.ImageUrl,
                    PublishDate = x.PublishDate
                })
                .ToArrayAsync();
        }
    }
}