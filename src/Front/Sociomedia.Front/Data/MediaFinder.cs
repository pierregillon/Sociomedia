using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sociomedia.DomainEvents.Media;
using Sociomedia.Front.Models;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.Front.Data
{
    public class MediaFinder
    {
        private readonly DbConnectionReadModel _dbConnection;

        public MediaFinder(DbConnectionReadModel dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IReadOnlyCollection<MediaListItem>> List()
        {
            await Task.Delay(0);

            return new[] {
                new MediaListItem {
                    Id = Guid.NewGuid(),
                    Name = "Libération",
                    PoliticalOrientation = "droite",
                    ImageUrl = "https://statics.liberation.fr/newsite/images/logo-libe.svg"
                },
                new MediaListItem {
                    Id = Guid.NewGuid(),
                    Name = "Marianne",
                    PoliticalOrientation = "gauche",
                    ImageUrl = "https://www.marianne.net/sites/default/themes/marianne/images/logo-marianne.svg"
                },
            };
        }

        public async Task<ArticleViewModel> Get(Guid mediaId)
        {
            await Task.Delay(0);
            return new ArticleViewModel {
                Id = mediaId,
                Name = "Marianne",
                Feeds = { new FeedItem { Id = 1, Url = "test" }, new FeedItem { Id = 2, Url = "test2" } },
                PoliticalOrientation = PoliticalOrientation.ExtremeRight,
                ImageUrl = "qsdfjqsjfjqsdfj"
            };
        }
    }
}