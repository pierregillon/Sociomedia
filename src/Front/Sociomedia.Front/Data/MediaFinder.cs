using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
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
            return await _dbConnection.Medias
                .Select(x => new MediaListItem {
                    Id = x.Id,
                    Name = x.Name,
                    ImageUrl = x.ImageUrl
                })
                .ToArrayAsync();
        }

        public async Task<ArticleViewModel> Get(Guid mediaId)
        {
            var media = await _dbConnection.Medias
                .Where(x => x.Id == mediaId)
                .Select(media => new {
                    Id = media.Id,
                    Name = media.Name,
                    ImageUrl = media.ImageUrl,
                    PoliticalOrientation = (PoliticalOrientation) media.PoliticalOrientation
                })
                .SingleOrDefaultAsync();

            if (media != null) {
                var feeds = await _dbConnection.MediaFeeds
                    .Where(x => x.MediaId == mediaId)
                    .Select((x, i) => new FeedItem {
                        Url = x.FeedUrl,
                        Id = i + 1
                    }).ToListAsync();

                return new ArticleViewModel {
                    Id = media.Id,
                    Name = media.Name,
                    ImageUrl = media.ImageUrl,
                    PoliticalOrientation = media.PoliticalOrientation,
                    Feeds = feeds
                };
            }

            return null;
        }
    }
}