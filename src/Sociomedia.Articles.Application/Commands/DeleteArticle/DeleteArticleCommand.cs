using System;
using Sociomedia.Core.Application;

namespace Sociomedia.Articles.Application.Commands.DeleteArticle
{
    public class DeleteArticleCommand : ICommand
    {
        public Guid ArticleId { get; }

        public DeleteArticleCommand(Guid articleId)
        {
            ArticleId = articleId;
        }
    }
}