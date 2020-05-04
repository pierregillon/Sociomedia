using System;
using LinqToDB.Mapping;

namespace NewsAggregator.Front.Data.ReadModels.Tables
{
    [Table(Name = "Articles")]
    public class ArticleTable
    {
        [PrimaryKey] public Guid Id { get; set; }

        [Column] [NotNull] public string Title { get; set; }

        [Column] [NotNull] public string Url { get; set; }
    }
}