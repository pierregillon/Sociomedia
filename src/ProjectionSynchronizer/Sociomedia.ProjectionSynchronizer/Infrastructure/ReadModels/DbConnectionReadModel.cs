using LinqToDB;
using Sociomedia.ProjectionSynchronizer.Infrastructure.ReadModels.Tables;

namespace Sociomedia.ProjectionSynchronizer.Infrastructure.ReadModels
{
    public class DbConnectionReadModel : LinqToDB.Data.DataConnection
    {
        public DbConnectionReadModel() : base("Sociomedia.Synchronizer") { }

        public ITable<ArticleTable> Articles => GetTable<ArticleTable>();
        public ITable<KeywordTable> Keywords => GetTable<KeywordTable>();
        public ITable<SynchronizationInformationTable> SynchronizationInformation => GetTable<SynchronizationInformationTable>();
    }
}