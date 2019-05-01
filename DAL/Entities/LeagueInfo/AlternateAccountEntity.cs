using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("AlternateAccount")]
    public class AlternateAccountEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid SummonerId { get; set; }
        public string AlternateName { get; set; }
        public string OpGGUrlLink { get; set; }
    }
}
