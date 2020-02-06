using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;

namespace DAL.Stores.TSR
{
    public class MatchDetailStore : TableStorageRepository<MatchDetailEntity>
    {
        public MatchDetailStore(IDatabase database) : base(database)
        {
        }
    }
}
