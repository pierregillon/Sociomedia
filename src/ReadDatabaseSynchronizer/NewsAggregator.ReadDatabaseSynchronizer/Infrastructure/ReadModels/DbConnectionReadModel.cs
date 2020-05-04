using LinqToDB;
using NewsAggregator.ReadDatabaseSynchronizer.Infrastructure.ReadModels.Tables;

namespace NewsAggregator.ReadDatabaseSynchronizer.Infrastructure.ReadModels
{
    public class DbConnectionReadModel : LinqToDB.Data.DataConnection
    {
        public DbConnectionReadModel() : base("NewsAggregator") { }

        public ITable<ArticleTable> Articles => GetTable<ArticleTable>();
        public ITable<KeywordTable> Keywords => GetTable<KeywordTable>();
        public ITable<SynchronizationInformationTable> SynchronizationInformation => GetTable<SynchronizationInformationTable>();
    }
}