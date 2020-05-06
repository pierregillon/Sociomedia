using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public interface IRssSourceReader
    {
        Task<IReadOnlyCollection<ExternalArticle>> ReadNewArticles(Uri url, DateTimeOffset? from);
    }
}