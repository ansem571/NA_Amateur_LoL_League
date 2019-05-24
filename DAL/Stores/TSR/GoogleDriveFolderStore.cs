using System;
using System.Collections.Generic;
using System.Text;
using DAL.Data.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.Logging;

namespace DAL.Stores.TSR
{
    public class GoogleDriveFolderStore : TableStorageRepository<GoogleDriveFolderEntity>
    {
        public GoogleDriveFolderStore(IDatabase database) : base(database)
        {
        }
    }
}
