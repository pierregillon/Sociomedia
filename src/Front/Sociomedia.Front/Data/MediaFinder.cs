using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}