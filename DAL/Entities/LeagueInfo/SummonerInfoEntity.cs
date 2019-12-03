using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("SummonerInfo")]
    public class SummonerInfoEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string SummonerName { get; set; }
        public Guid RoleId { get; set; }
        public Guid Tier_DivisionId { get; set; }
        public string OpGGUrlLink { get; set; }
        public bool IsValidPlayer { get; set; }
        public int CurrentLp { get; set; }
        public Guid? OffRoleId { get; set; }
        public bool? IsSubOnly { get; set; }
        public Guid? TeamRoleId { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
