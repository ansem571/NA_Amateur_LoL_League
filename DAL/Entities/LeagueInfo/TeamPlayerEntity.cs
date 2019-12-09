using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("TeamPlayer")]
    public class TeamPlayerEntity
    {
        [Key]
        public Guid SummonerId { get; set; }
        [Key]
        public Guid TeamRosterId { get; set; }
        public bool? IsSub { get; set; }
        public Guid? SeasonInfoId { get; set; }
    }
}
