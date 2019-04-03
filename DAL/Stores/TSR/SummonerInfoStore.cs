using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;

namespace DAL.Stores.TSR
{  
    public class SummonerInfoStore : TableStorageRepository<SummonerInfoEntity>
    {
        public SummonerInfoStore(IDatabase database) : base(database)
        {
        }
    }
}
