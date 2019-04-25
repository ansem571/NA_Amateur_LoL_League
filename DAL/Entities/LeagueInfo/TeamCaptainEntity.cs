using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("TeamCaptain")]
    public class TeamCaptainEntity
    {
        public Guid SummonerId { get; set; }
        public Guid TeamRosterId { get; set; }
    }
}
