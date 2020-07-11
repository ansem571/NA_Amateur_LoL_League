using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("SeasonInfo")]
    public class SeasonInfoEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string SeasonName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ClosedRegistrationDate { get; set; }
        public DateTime SeasonStartDate { get; set; }
        public DateTime? SeasonEndDate { get; set; }
        public bool IsCurrentSeason { get; set; }
    }
}
