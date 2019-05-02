using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("PlayerStats")]
    public class PlayerStatsEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid SummonerId { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int CS { get; set; }
        public double Damage { get; set; }
        public int TotalTeamKills { get; set; }
        public int VisionScore { get; set; }
        public int GameTime { get; set; }
    }
}
