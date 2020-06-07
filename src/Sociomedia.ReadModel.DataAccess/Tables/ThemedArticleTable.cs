using System;
using LinqToDB.Mapping;

namespace Sociomedia.ReadModel.DataAccess.Tables
{
    [Table(Name = "ThemedArticles")]
    public class ThemedArticleTable
    {
        [PrimaryKey] public Guid ThemeId { get; set; }
        [PrimaryKey] public Guid ArticleId { get; set; }
    }
}