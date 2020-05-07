using System;
using LinqToDB.Mapping;

namespace Sociomedia.ReadModel.DataAccess.Tables
{
    [Table(Name = "Articles")]
    public class ArticleTable
    {
        [PrimaryKey] public Guid Id { get; set; }

        [Column] [NotNull] public string Title { get; set; }

        [Column] public string Summary { get; set; }

        [Column] [NotNull] public string Url { get; set; }

        [Column] [NotNull] public DateTimeOffset PublishDate { get; set; }

        [Column] public string ImageUrl { get; set; }

        [Column]  public Guid MediaId { get; set; }
    }
}