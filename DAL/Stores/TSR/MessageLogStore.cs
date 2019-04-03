using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.Logging;

namespace DAL.Stores.TSR
{
    public class MessageLogStore : TableStorageRepository<MessageLogEntity>
    {
        public MessageLogStore(IDatabase database) : base(database)
        {
        }
    }
}
