using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("MatchDetail")]
    public class MatchDetailEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid TeamScheduleId { get; set; }
        public int Game { get; set; }
        public Guid PlayerStatsId { get; set; }
        public Guid PlayerId { get; set; }
        public Guid SeasonInfoId { get; set; }
        public bool Winner { get; set; }
    }
}
