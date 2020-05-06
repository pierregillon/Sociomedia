using System;
using LinqToDB.Mapping;

namespace Sociomedia.ReadModel.DataAccess.Tables
{
    [Table(Name = "SynchronizationInformation")]
    public class SynchronizationInformationTable
    {
        [Column] [PrimaryKey] public long LastPosition { get; set; }
        [Column] [PrimaryKey] public DateTime? LastUpdateDate { get; set; }
    }
}