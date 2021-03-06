﻿using System;
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

        public static PlayerStatsEntity operator +(PlayerStatsEntity current, PlayerStatsEntity other)
        {
            if (current.SummonerId != other.SummonerId || current.SeasonInfoId != other.SeasonInfoId)
            {
                throw new Exception("Invalid summoner stats merge");
            }
            return new PlayerStatsEntity
            {
                SummonerId = current.SummonerId,
                Kills = current.Kills + other.Kills,
                Deaths = current.Deaths + other.Deaths,
                Assists = current.Assists + other.Assists,
                CS = current.CS + other.CS,
                Gold = current.Gold + other.Gold,
                TotalTeamKills = current.TotalTeamKills + other.TotalTeamKills,
                VisionScore = current.VisionScore + other.VisionScore,
                GameTime = current.GameTime + other.GameTime,
                Games = current.Games + 1,
                SeasonInfoId = current.SeasonInfoId
            };
        }
    }
}
