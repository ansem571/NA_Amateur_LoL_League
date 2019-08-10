using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;
using DAL.Contracts;

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
        public int Gold { get; set; }
        public int TotalTeamKills { get; set; }
        public int VisionScore { get; set; }
        public TimeSpan GameTime { get; set; }
        public int Games { get; set; }
        public Guid? SeasonInfoId { get; set; }

        public PlayerStatsEntity() { }

        public PlayerStatsEntity(PartialPlayerInfo newStats, Guid summonerId, Guid seasonInfoId)
        {
            Id = Guid.NewGuid();
            SummonerId = summonerId;
            Kills = newStats.Kills;
            Deaths = newStats.Deaths;
            Assists = newStats.Assists;
            CS = newStats.CS;
            Gold = newStats.Gold;
            VisionScore = newStats.RealVisionScore;
            GameTime = newStats.Duration;
            TotalTeamKills = newStats.TotalTeamKills;
            Games = newStats.Games;
            SeasonInfoId = seasonInfoId;
        }

        public void UpdateValues(PartialPlayerInfo newStats)
        {
            Kills = newStats.Kills;
            Deaths = newStats.Deaths;
            Assists = newStats.Assists;
            CS = newStats.CS;
            Gold = newStats.Gold;
            VisionScore = newStats.RealVisionScore;
            GameTime = newStats.Duration;
            TotalTeamKills = newStats.TotalTeamKills;
            Games = newStats.Games;
        }
    }
}
