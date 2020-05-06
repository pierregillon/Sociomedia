using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ReadModel.DataAccess;
using Sociomedia.ReadModel.DataAccess.Tables;

namespace Sociomedia.ProjectionSynchronizer.Infrastructure
{
    public class StreamPositionRepository : IStreamPositionRepository
    {
        private readonly DbConnectionReadModel _dbConnection;

        public StreamPositionRepository(DbConnectionReadModel dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task Save(long position)
        {
            if (await _dbConnection.SynchronizationInformation.AnyAsync()) {
                await _dbConnection.SynchronizationInformation
                    .Set(x => x.LastPosition, position)
                    .Set(x => x.LastUpdateDate, DateTime.Now)
                    .UpdateAsync();
            }
            else {
                await _dbConnection.SynchronizationInformation
                    .InsertAsync(() => new SynchronizationInformationTable {
                        LastPosition = position,
                        LastUpdateDate = DateTime.Now
                    });
            }
        }

        public async Task<long?> GetLastPosition()
        {
            return await _dbConnection.SynchronizationInformation
                .Select(x => x.LastPosition)
                .SingleOrDefaultAsync();
        }
    }
}