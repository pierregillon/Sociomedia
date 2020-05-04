using System;
using LinqToDB.Mapping;

namespace NewsAggregator.Front.Data.ReadModels.Tables {
    [Table(Name = "Keywords")]
    public class KeywordTable
    {
        [PrimaryKey] [Identity] public int Id { get; set; }

        [NotNull] [Column] public Guid FK_Article { get; set; }

        [Column] public string Value { get; set; }
    }
}