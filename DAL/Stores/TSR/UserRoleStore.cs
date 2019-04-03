using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.UserData;

namespace DAL.Stores.TSR
{
    public class UserRoleStore : TableStorageRepository<UserRoleRelationEntity>
    {
        public UserRoleStore(IDatabase database) : base(database)
        {
        }
    }
}
