using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsAggregator.Application.Queries {
    public interface IArticleFinder
    {
        Task<IReadOnlyCollection<ArticleReadModel>> GetArticles(Guid rssSourceId);
    }
}