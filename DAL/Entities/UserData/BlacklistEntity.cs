﻿using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.UserData
{
    [Table("Blacklist")]
    public class BlacklistEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool IsBanned { get; set; }
    }
}
