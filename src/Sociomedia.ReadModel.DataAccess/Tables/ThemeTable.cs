using System;
using LinqToDB.Mapping;

namespace Sociomedia.ReadModel.DataAccess.Tables
{
    [Table(Name = "Themes")]
    public class ThemeTable
    {
        [PrimaryKey] public Guid Id { get; set; }
        [NotNull] [Column] public string Name { get; set; }
        [NotNull] [Column] public string FullKeywords { get; set; }
        [NotNull] [Column] public int KeywordCount { get; set; }
        [NotNull] [Column(Precision = 2)] public double OccurencePerKeywordPerArticle { get; set; }
    }
}