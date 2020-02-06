using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;

namespace DAL.Stores.TSR
{
    public class PlayerStatsStore : TableStorageRepository<PlayerStatsEntity>
    {
        public PlayerStatsStore(IDatabase database) : base(database)
        {
        }
    }
}
