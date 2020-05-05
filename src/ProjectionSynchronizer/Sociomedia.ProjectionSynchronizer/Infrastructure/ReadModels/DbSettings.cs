using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;

namespace Sociomedia.ProjectionSynchronizer.Infrastructure.ReadModels {
    public class DbSettings : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "SqlServer";
        public string DefaultDataProvider => "SqlServer";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get {
                yield return
                    new ConnectionStringSettings {
                        Name = "Sociomedia",
                        ProviderName = "SqlServer",
                        ConnectionString = @"Server=.\SQLEXPRESS;Database=Sociomedia;Trusted_Connection=True;Enlist=False;"
                    };
            }
        }
    }
}