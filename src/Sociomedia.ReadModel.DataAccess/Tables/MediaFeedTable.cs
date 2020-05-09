using System;
using LinqToDB.Mapping;

namespace Sociomedia.ReadModel.DataAccess.Tables
{
    [Table(Name = "MediaFeeds")]
    public class MediaFeedTable
    {
        [NotNull] [Column] public Guid MediaId { get; set; }
        [NotNull] [Column] public string FeedUrl { get; set; }
        [Column]  public DateTimeOffset? SynchronizationDate { get; set; }
    }
}