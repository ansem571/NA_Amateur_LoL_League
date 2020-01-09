using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("MatchMvp")]
    public class MatchMvpEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid? BlueMvp { get; set; }
        public Guid? RedMvp { get; set; }
        public Guid TeamScheduleId { get; set; }
        public int Game { get; set; }
    }
}
