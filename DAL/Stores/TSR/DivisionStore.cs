using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;

namespace DAL.Stores.TSR
{
    public class DivisionStore : TableStorageRepository<DivisionEntity>
    {
        public DivisionStore(IDatabase database) : base(database)
        {
        }
    }
}
