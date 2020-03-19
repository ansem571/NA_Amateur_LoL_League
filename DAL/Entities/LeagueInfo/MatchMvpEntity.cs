using System;
using Dapper.Contrib.Extensions;
using DAL.Contracts;

namespace DAL.Entities.LeagueInfo
{
    [Table("MatchMvp")]
    public class MatchMvpEntity : BaseInfo
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid? BlueMvp { get; set; }
        public Guid? RedMvp { get; set; }
        public Guid TeamScheduleId { get; set; }
        public int Game { get; set; }
    }
}
