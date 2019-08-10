using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("TeamRoster")]
    public class TeamRosterEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string TeamName { get; set; }
        public int? TeamTierScore { get; set; }
        public int? Wins { get; set; }
        public int? Loses { get; set; }
        public int? Points { get; set; }
        public Guid? SeasonInfoId { get; set; }
    }
}
