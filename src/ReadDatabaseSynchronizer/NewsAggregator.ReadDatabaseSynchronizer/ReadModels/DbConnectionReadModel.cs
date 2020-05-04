using LinqToDB;

namespace NewsAggregator.ReadDatabaseSynchronizer.ReadModels {
    public class DbConnectionReadModel : LinqToDB.Data.DataConnection
    {
        public DbConnectionReadModel() : base("NewsAggregator") { }

        public ITable<ArticleTable> Articles => GetTable<ArticleTable>();
        public ITable<KeywordTable> Keywords => GetTable<KeywordTable>();
    }
}