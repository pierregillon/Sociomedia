using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.Domain.Medias;
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

        public async Task<MediaDetailDto> GetDetails(Guid mediaId)
        {
            var mediaDetail = await _dbConnection.Medias
                .Where(x => x.Id == mediaId)
                .Select(media => new {
                    Id = media.Id,
                    Name = media.Name,
                    ImageUrl = media.ImageUrl,
                    PoliticalOrientation = (PoliticalOrientation) media.PoliticalOrientation
                })
                .SingleOrDefaultAsync();

            var articles = from article in _dbConnection.Articles
                join media in _dbConnection.Medias on article.MediaId equals media.Id
                orderby article.PublishDate descending
                where media.Id == mediaId
                select new ArticleListItem {
                    Id = article.Id,
                    Title = article.Title,
                    Url = article.Url,
                    Summary = article.Summary,
                    ImageUrl = article.ImageUrl,
                    PublishDate = article.PublishDate,
                    MediaId = article.MediaId,
                    MediaImageUrl = media.ImageUrl
                };

            return new MediaDetailDto {
                Name = mediaDetail.Name,
                ImageUrl = mediaDetail.ImageUrl,
                PoliticalOrientation = mediaDetail.PoliticalOrientation,
                Articles = await articles.ToArrayAsync()
            };
        }
    }

    public class MediaDetailDto
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public PoliticalOrientation PoliticalOrientation { get; set; }
        public IReadOnlyCollection<ArticleListItem> Articles { get; set; }
    }
}