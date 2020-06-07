using LinqToDB;
using Sociomedia.ReadModel.DataAccess.Tables;

namespace Sociomedia.ReadModel.DataAccess
{
    public class DbConnectionReadModel : LinqToDB.Data.DataConnection
    {
        public DbConnectionReadModel() : base("Sociomedia.Synchronizer") { }

        public ITable<ArticleTable> Articles => GetTable<ArticleTable>();
        public ITable<SynchronizationInformationTable> SynchronizationInformation => GetTable<SynchronizationInformationTable>();
        public ITable<MediaTable> Medias => GetTable<MediaTable>();
        public ITable<MediaFeedTable> MediaFeeds => GetTable<MediaFeedTable>();
        public ITable<ThemeTable> Themes => GetTable<ThemeTable>();
        public ITable<ThemedArticleTable> ThemedArticles => GetTable<ThemedArticleTable>();
    }
}