using System;
using System.Threading.Tasks;
using Sociomedia.DomainEvents.Article;
using Sociomedia.ReadModel.DataAccess.Tables;

namespace Sociomedia.ProjectionSynchronizer.Application.EventListeners
{
    public class ArticleTableSynchronizer : IEventListener<ArticleSynchronized>
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleTableSynchronizer(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task On(ArticleSynchronized @event)
        {
            await _articleRepository.AddArticle(new ArticleTable {
                Id = @event.Id,
                Title = @event.Title,
                Url = @event.Url,
                Summary = @event.Summary,
                ImageUrl = @event.ImageUrl,
                PublishDate = @event.PublishDate
            });

            foreach (var keyword in @event.Keywords) {
                await _articleRepository.AddKeywords(new KeywordTable {
                    FK_Article = @event.Id,
                    Value = keyword.Substring(0, Math.Min(keyword.Length, 50))
                });
            }
        }
    }
}