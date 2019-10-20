using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("Achievement")]
    public class AchievementEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Achievement { get; set; }
        public DateTime AchievedDate { get; set; }
        public string AchievedTeam { get; set; }
        public string AchievedAgainst { get; set; }
    }
}
