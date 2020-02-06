using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("PlayoffSeed")]
    public class PlayoffSeedEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid RosterId { get; set; }
        public Guid DivisionId { get; set; }
        public Guid SeasonInfoId { get; set; }
        public int Seed { get; set; }
        public int PlayoffBracket { get; set; }
    }
}
