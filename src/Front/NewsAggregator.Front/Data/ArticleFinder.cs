using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using NewsAggregator.Front.Data.ReadModels;

namespace NewsAggregator.Front.Data
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
                .Select(x => new ArticleListItem {
                    Id = x.Id,
                    Title = x.Title,
                    Url = x.Url,
                    Description = "Les macronistes et la gauche s’accusent mutuellement d’avoir voté contre la mutualisation des dettes européennes pour faire face à la crise. Décryptage d’un imbroglio.",
                    ImageUrl = "https://img.lemde.fr/2020/05/03/2/0/4194/2796/384/0/60/0/b047450_PNNzTd_HJmTswhYsnUroDwyh.jpg"
                })
                .ToArrayAsync();
        }
    }
}