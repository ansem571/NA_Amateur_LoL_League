using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("TeamSchedule")]
    public class ScheduleEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid SeasonInfoId { get; set; }
        public Guid HomeRosterTeamId { get; set; }
        public Guid AwayRosterTeamId { get; set; }
        public int HomeTeamWins { get; set; }
        public int AwayTeamWins { get; set; }
        public DateTime MatchWeek { get; set; }
        public DateTime? MatchScheduledTime { get; set; }
        public string CasterName { get; set; }
    }
}
