using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.UserData;

namespace DAL.Stores
{
    public class BlacklistStore : TableStorageRepository<BlacklistEntity>
    {
        public BlacklistStore(IDatabase database) : base(database)
        {
        }
    }
}
