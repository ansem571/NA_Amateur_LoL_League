using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;

namespace DAL.Stores.TSR
{
    public class TeamCaptainStore : TableStorageRepository<TeamCaptainEntity>
    {
        public TeamCaptainStore(IDatabase database) : base(database)
        {
        }
    }
}
