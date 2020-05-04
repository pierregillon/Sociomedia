using System;
using LinqToDB.Mapping;

namespace NewsAggregator.ReadDatabaseSynchronizer.ReadModels
{
    [Table(Name = "SynchronizationInformation")]
    public class SynchronizationInformationTable
    {
        [Column] public long? LastEventId { get; set; }
        [Column] public DateTime? LastUpdateDate { get; set; }
    }
}