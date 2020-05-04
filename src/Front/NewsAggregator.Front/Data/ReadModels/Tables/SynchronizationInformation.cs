using System;
using LinqToDB.Mapping;

namespace NewsAggregator.Front.Data.ReadModels.Tables
{
    [Table(Name = "SynchronizationInformation")]
    public class SynchronizationInformationTable
    {
        [Column] [PrimaryKey] public long LastCommitPosition { get; set; }
        [Column] [PrimaryKey] public long LastPreparePosition { get; set; }
        [Column] [PrimaryKey] public DateTime? LastUpdateDate { get; set; }
    }
}