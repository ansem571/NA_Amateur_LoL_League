using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.UserData;

namespace DAL.Stores.TSR
{
    public class LookupStore : TableStorageRepository<LookupEntity>
    {
        public LookupStore(IDatabase database) : base(database)
        {
        }
    }
}
