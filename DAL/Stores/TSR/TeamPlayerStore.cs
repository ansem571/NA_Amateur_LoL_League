using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;

namespace DAL.Stores.TSR
{
    public class TeamPlayerStore : TableStorageRepository<TeamPlayerEntity>
    {
        public TeamPlayerStore(IDatabase database) : base(database)
        {
        }
    }
}
