using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator.Domain.Medias
{
    public interface IFeedReader
    {
        Task<IReadOnlyCollection<ExternalArticle>> ReadNewArticles(string url, DateTimeOffset? from);
    }
}