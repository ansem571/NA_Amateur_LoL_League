using System;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.UserData
{
    [Table("UserRoleRelation")]
    public class UserRoleRelationEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
