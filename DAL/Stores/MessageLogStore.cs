using System;
using System.Collections.Generic;
using System.Text;
using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.Logging;
using Microsoft.AspNetCore.Identity;

namespace DAL.Stores
{
    public class MessageLogStore : TableStorageRepository<MessageLogEntity>
    {
        public MessageLogStore(IDatabase database) : base(database)
        {
        }
    }
}
