using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.UserData
{
    [Table("Lookup")]
    public class LookupEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Enum { get; set; }
        public string Value { get; set; }
    }
}
