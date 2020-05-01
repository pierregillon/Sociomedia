using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAggregator.Application.Queries {
    public class ArticleFinder : IArticleFinder
    {
        private readonly ReadModelDatabase _database;

        public ArticleFinder(ReadModelDatabase database)
        {
            _database = database;
        }

        public Task<IReadOnlyCollection<ArticleReadModel>> GetArticles(Guid rssSourceId)
        {
            return Task.FromResult((IReadOnlyCollection<ArticleReadModel>) _database.Articles.Where(x => x.RssSourceId == rssSourceId).ToArray());
        }
    }
}