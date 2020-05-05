using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;

namespace Sociomedia.Front.Data.ReadModels
{
    public class DbSettings : ILinqToDBSettings
    {
        private readonly string _providerName;
        private readonly string _connectionString;

        public DbSettings(string providerName, string connectionString)
        {
            _providerName = providerName ?? throw new ArgumentNullException(nameof(providerName), "Provider name is mandatory to contact Sql database");
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString), "Connection string is mandatory to contact Sql database");
        }

        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => _providerName;
        public string DefaultDataProvider => _providerName;

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get {
                yield return
                    new ConnectionStringSettings {
                        Name = "Sociomedia.Front",
                        ProviderName = _providerName,
                        ConnectionString = _connectionString
                    };
            }
        }
    }
}