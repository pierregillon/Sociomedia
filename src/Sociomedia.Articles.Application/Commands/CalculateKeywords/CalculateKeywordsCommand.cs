using System;
using Sociomedia.Core.Application;

namespace Sociomedia.Articles.Application.Commands.CalculateKeywords
{
    public class CalculateKeywordsCommand : ICommand
    {
        public CalculateKeywordsCommand(Guid articleId)
        {
            ArticleId = articleId;
        }

        public Guid ArticleId { get; }
    }
}