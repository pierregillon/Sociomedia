using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;

namespace Sociomedia.ReadModel.DataAccess {
    public class DbSettings : ILinqToDBSettings
    {
        private readonly SqlDatabaseConfiguration _configuration;

        public DbSettings(SqlDatabaseConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => _configuration.ProviderName;
        public string DefaultDataProvider => _configuration.ProviderName;

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get {
                yield return
                    new ConnectionStringSettings {
                        Name = "Sociomedia.Synchronizer",
                        ProviderName = _configuration.ProviderName,
                        ConnectionString = _configuration.ConnectionString
                    };
            }
        }
    }
}