using LinqToDB;
using NewsAggregator.Front.Data.ReadModels.Tables;

namespace NewsAggregator.Front.Data.ReadModels
{
    public class DbConnectionReadModel : LinqToDB.Data.DataConnection
    {
        public DbConnectionReadModel() : base("NewsAggregator") { }

        public ITable<ArticleTable> Articles => GetTable<ArticleTable>();
        public ITable<KeywordTable> Keywords => GetTable<KeywordTable>();
        public ITable<SynchronizationInformationTable> SynchronizationInformation => GetTable<SynchronizationInformationTable>();
    }
}