using LinqToDB;
using Sociomedia.ReadModel.DataAccess.Tables;

namespace Sociomedia.ReadModel.DataAccess
{
    public class DbConnectionReadModel : LinqToDB.Data.DataConnection
    {
        public DbConnectionReadModel() : base("Sociomedia.Synchronizer") { }

        public ITable<ArticleTable> Articles => GetTable<ArticleTable>();
        public ITable<KeywordTable> Keywords => GetTable<KeywordTable>();
        public ITable<SynchronizationInformationTable> SynchronizationInformation => GetTable<SynchronizationInformationTable>();
    }
}