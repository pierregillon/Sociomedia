using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsAggregator.Domain.Rss
{
    public interface IRssSourceReader
    {
        Task<IReadOnlyCollection<ExternalArticle>> ReadNewArticles(Uri url, DateTimeOffset? from);
    }
}