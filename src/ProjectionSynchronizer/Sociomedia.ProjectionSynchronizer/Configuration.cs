using Sociomedia.ReadModel.DataAccess;

namespace Sociomedia.ProjectionSynchronizer
{
    public class Configuration
    {
        public EventStoreConfiguration EventStore { get; set; }

        public SqlDatabaseConfiguration SqlDatabase { get; set; }
    }

    public class EventStoreConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}