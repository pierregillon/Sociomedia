using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsAggregator.Domain.Articles {
    public interface IArticleRepository
    {
        Task<IReadOnlyCollection<Article>> GetAll(string rssSourceId);
        Task Save(Article article);
    }
}