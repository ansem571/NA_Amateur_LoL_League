using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.UserData
{
    [Table("UserRole")]
    public class UserRoleEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}
