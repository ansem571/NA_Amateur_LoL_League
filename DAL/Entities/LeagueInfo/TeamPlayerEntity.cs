using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("TeamPlayer")]
    public class TeamPlayerEntity
    {
        public Guid SummonerId { get; set; }
        public Guid TeamRosterId { get; set; }
    }
}
