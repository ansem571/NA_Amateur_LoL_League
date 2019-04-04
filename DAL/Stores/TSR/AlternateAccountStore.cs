using System;
using System.Collections.Generic;
using System.Text;
using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;

namespace DAL.Stores.TSR
{
    public class AlternateAccountStore : TableStorageRepository<AlternateAccountEntity>
    {
        public AlternateAccountStore(IDatabase database) : base(database)
        {
        }
    }
}
