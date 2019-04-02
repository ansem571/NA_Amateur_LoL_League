using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.UserData
{
    [Table("User")]
    public class UserEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool ConfirmedEmail { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Guid? PhoneCarrierId { get; set; }


        public override string ToString()
        {
            return $"Id: {Id} Email: {Email} Password: {PasswordHash}";
        }
    }
}
