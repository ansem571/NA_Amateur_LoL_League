using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;

namespace DAL.Stores.TSR
{
    public class ScheduleStore : TableStorageRepository<ScheduleEntity>
    {
        public ScheduleStore(IDatabase database) : base(database)
        {
        }
    }
}
