using System;
using System.Collections.Generic;
using System.Text;
using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;

namespace DAL.Stores.TSR
{
    public class PlayoffSeedStore : TableStorageRepository<PlayoffSeedEntity>
    {
        public PlayoffSeedStore(IDatabase database) : base(database)
        {
        }
    }
}
