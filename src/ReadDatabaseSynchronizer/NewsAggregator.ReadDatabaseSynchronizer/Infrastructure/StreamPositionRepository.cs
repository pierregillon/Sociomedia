using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using LinqToDB;
using NewsAggregator.ReadDatabaseSynchronizer.Application;
using NewsAggregator.ReadDatabaseSynchronizer.Infrastructure.ReadModels;
using NewsAggregator.ReadDatabaseSynchronizer.Infrastructure.ReadModels.Tables;

namespace NewsAggregator.ReadDatabaseSynchronizer.Infrastructure
{
    public class StreamPositionRepository : IStreamPositionRepository
    {
        private readonly DbConnectionReadModel _dbConnection;

        public StreamPositionRepository(DbConnectionReadModel dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task Save(Position position)
        {
            if (await _dbConnection.SynchronizationInformation.AnyAsync()) {
                await _dbConnection.SynchronizationInformation
                    .Set(x => x.LastCommitPosition, position.CommitPosition)
                    .Set(x => x.LastPreparePosition, position.PreparePosition)
                    .Set(x => x.LastUpdateDate, DateTime.Now)
                    .UpdateAsync();
            }
            else {
                await _dbConnection.SynchronizationInformation
                    .InsertAsync(() => new SynchronizationInformationTable {
                        LastCommitPosition = position.CommitPosition,
                        LastPreparePosition = position.PreparePosition,
                        LastUpdateDate = DateTime.Now
                    });
            }
        }

        public async Task<Position?> GetLastPosition()
        {
            return await _dbConnection.SynchronizationInformation
                .Select(x => new Position(x.LastCommitPosition, x.LastPreparePosition))
                .SingleOrDefaultAsync();
        }
    }
}