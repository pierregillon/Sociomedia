using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sociomedia.Domain.Articles;

namespace Sociomedia.Domain.Medias
{
    public interface IFeedReader
    {
        Task<IReadOnlyCollection<ExternalArticle>> ReadArticles(string url);
    }
}