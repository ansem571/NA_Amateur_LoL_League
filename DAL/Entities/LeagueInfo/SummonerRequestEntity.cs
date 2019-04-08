using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("SummonerRequest")]
    public class SummonerRequestEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid SummonerId { get; set; }
        public Guid SummonerRequestedId { get; set; }
        public bool IsSub { get; set; }
    }
}
