using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;

namespace DAL.Stores.TSR
{
    public class ChampionStatsStore : TableStorageRepository<ChampionStatsEntity>
    {
        public ChampionStatsStore(IDatabase database) : base(database)
        {
        }
    }
}
