using System;
using System.IO;
using LinqToDB.Data;
using Sociomedia.ReadModel.DataAccess;
using StructureMap;

namespace Sociomedia.ProjectionSynchronizer.Tests
{
    public class AcceptanceTests : IDisposable
    {
        protected readonly IContainer Container;
        private readonly string _databaseName = "database-" + Guid.NewGuid() + ".sqlite";
        private string FullPath => Path.Combine(Directory.GetCurrentDirectory(), _databaseName);

        public AcceptanceTests()
        {
            File.Copy("./database.db", FullPath);

            Container = ContainerBuilder.Build(new Configuration {
                SqlDatabase = new SqlDatabaseConfiguration {
                    ProviderName = "SQLite",
                    ConnectionString = "Data Source=" + FullPath
                }
            });

            DataConnection.DefaultSettings = Container.GetInstance<DbSettings>();

            InitDatabase();
        }

        public void Dispose()
        {
            Container?.Dispose();
            File.Delete(FullPath);
        }

        private void InitDatabase()
        {
            var db = Container.GetInstance<DbConnectionReadModel>();

            db.GenerateMissingTables();
            db.ClearDatabase();
        }
    }
}