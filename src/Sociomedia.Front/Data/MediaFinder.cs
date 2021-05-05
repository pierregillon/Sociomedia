using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.Front.Models;
using Sociomedia.Medias.Domain;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.Front.Data
{
    public class MediaFinder
    {
        public async Task<IReadOnlyCollection<MediaListItem>> List()
        {
            await using var dbConnection = new DbConnectionReadModel();
            
            return await dbConnection.Medias
                .Select(x => new MediaListItem {
                    Id = x.Id,
                    Name = x.Name,
                    ImageUrl = x.ImageUrl,
                    PoliticalOrientation = ((PoliticalOrientation)x.PoliticalOrientation).ToString()
                })
                .ToArrayAsync();
        }

        public async Task<ArticleViewModel> Get(Guid mediaId)
        {
            await using var dbConnection = new DbConnectionReadModel();
            
            var media = await dbConnection.Medias
                .Where(x => x.Id == mediaId)
                .Select(media => new {
                    Id = media.Id,
                    Name = media.Name,
                    ImageUrl = media.ImageUrl,
                    PoliticalOrientation = (PoliticalOrientation) media.PoliticalOrientation
                })
                .SingleOrDefaultAsync();

            if (media != null) {
                var feeds = await dbConnection.MediaFeeds
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

        public async Task<MediaDetailDto> GetDetails(Guid mediaId)
        {
            await using var dbConnection = new DbConnectionReadModel();
            
            var mediaDetail = await dbConnection.Medias
                .Where(x => x.Id == mediaId)
                .Select(media => new {
                    Id = media.Id,
                    Name = media.Name,
                    ImageUrl = media.ImageUrl,
                    PoliticalOrientation = (PoliticalOrientation) media.PoliticalOrientation
                })
                .SingleOrDefaultAsync();

            return new MediaDetailDto {
                Name = mediaDetail.Name,
                ImageUrl = mediaDetail.ImageUrl,
                PoliticalOrientation = mediaDetail.PoliticalOrientation
            };
        }
    }

    public class MediaDetailDto
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public PoliticalOrientation PoliticalOrientation { get; set; }
    }
}