using LinqToDB.Configuration;

namespace Sociomedia.ProjectionSynchronizer.Infrastructure.ReadModels {
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }
}