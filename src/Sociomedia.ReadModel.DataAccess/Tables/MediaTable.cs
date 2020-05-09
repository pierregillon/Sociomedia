using System;
using LinqToDB.Mapping;

namespace Sociomedia.ReadModel.DataAccess.Tables
{
    [Table(Name = "Medias")]
    public class MediaTable
    {
        [PrimaryKey] public Guid Id { get; set; }
        [NotNull] [Column] public string Name { get; set; }
        [Column] public string ImageUrl { get; set; }
        [NotNull] [Column] public int PoliticalOrientation { get; set; }
    }
}