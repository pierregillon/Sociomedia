using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;

namespace NewsAggregator.Front.Data.ReadModels {
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
                        Name = "NewsAggregator",
                        ProviderName = "SqlServer",
                        ConnectionString = @"Server=.\SQLEXPRESS;Database=NewsAggregator;Trusted_Connection=True;Enlist=False;"
                    };
            }
        }
    }
}