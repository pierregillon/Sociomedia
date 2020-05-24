using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.ProjectionSynchronizer.Application;
using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.ProjectionSynchronizer
{
    public class Configuration
    {
        public EventStoreConfiguration EventStore { get; set; } = new EventStoreConfiguration();

        public SqlDatabaseConfiguration SqlDatabase { get; set; } = new SqlDatabaseConfiguration();

        public ProjectionSynchronizationConfiguration ProjectionSynchronization { get; set; } = new ProjectionSynchronizationConfiguration();
    }
}