using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("Division")]
    public class DivisionEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid SeasonInfoId { get; set; }
        public string Name { get; set; }
        public int LowerLimit { get; set; }
        public int UpperLimit { get; set; }
    }
}
