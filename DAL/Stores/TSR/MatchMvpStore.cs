using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;

namespace DAL.Stores.TSR
{
    public class MatchMvpStore : TableStorageRepository<MatchMvpEntity>
    {
        public MatchMvpStore(IDatabase database) : base(database)
        {
        }
    }
}
