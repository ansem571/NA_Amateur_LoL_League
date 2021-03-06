﻿using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("ChampionStats")]
    public class ChampionStatsEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        /// <summary>
        /// Based on player from game? Wont be recorded if banned
        /// </summary>
        public Guid? PlayerId { get; set; }

        /// <summary>
        /// Will be the season from match
        /// </summary>
        public Guid SeasonInfoId { get; set; }

        /// <summary>
        /// Will be division from match
        /// </summary>
        public Guid DivisionId { get; set; }

        /// <summary>
        /// Will be ChampionId in Lookup Table
        /// </summary>
        public Guid ChampionId { get; set; }

        /// <summary>
        /// Will be the Match Detail, can be null such as for bans
        /// </summary>
        public Guid? MatchDetailId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid TeamScheduleId { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public bool Picked { get; set; }
        public bool Banned { get; set; }
        public bool Win { get; set; }
        public bool Loss { get; set; }
    }
}
