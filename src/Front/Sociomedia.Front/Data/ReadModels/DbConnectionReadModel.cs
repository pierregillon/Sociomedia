using LinqToDB;
using Sociomedia.Front.Data.ReadModels.Tables;

namespace Sociomedia.Front.Data.ReadModels
{
    public class DbConnectionReadModel : LinqToDB.Data.DataConnection
    {
        public DbConnectionReadModel() : base("Sociomedia") { }

        public ITable<ArticleTable> Articles => GetTable<ArticleTable>();
        public ITable<KeywordTable> Keywords => GetTable<KeywordTable>();
        public ITable<SynchronizationInformationTable> SynchronizationInformation => GetTable<SynchronizationInformationTable>();
    }
}