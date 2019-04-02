using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using DAL.Databases;
using DAL.Databases.Interfaces;

namespace DAL.Data.Interfaces
{
    public interface ITableStorageRepository<TEntity> where TEntity : class, new()
    {
        IDatabase Database { get; }

        Task<TEntity> ReadOneAsync(object id, IUnitOfWork unitOfWork = null, bool withNoLock = true);
        Task<IEnumerable<TEntity>> ReadManyAsync(string where = null, object param = null, string orderBy = null,
            int? top = null, int? page = null, int pageSize = 25, IEnumerable<string> cols = null, IUnitOfWork unitOfWork = null, bool withNoLock = true);
        Task<IEnumerable<TEntity>> ReadAllAsync(IUnitOfWork unitOfWork = null, bool withNoLock = true);

        Task<int> InsertAsync(TEntity item, IUnitOfWork unitOfWork = null);
        Task<int> InsertAsync(IEnumerable<TEntity> items, IUnitOfWork unitOfWork = null);

        Task<bool> UpdateAsync(TEntity item, IUnitOfWork unitOfWork = null);
        Task<bool> UpdateAsync(IEnumerable<TEntity> items, IUnitOfWork unitOfWork = null);

        Task<bool> DeleteAsync(TEntity item, IUnitOfWork unitOfWork = null);
        Task<bool> DeleteAsync(IEnumerable<TEntity> items, IUnitOfWork unitOfWork = null);
        Task<bool> DeleteAllAsync(IUnitOfWork unitOfWork = null);
        Task<bool> DeleteWhereAsync(string where, object param, IUnitOfWork unitOfWork = null);

        Task<int> BulkInsertAsync(IEnumerable<TEntity> items, int? bulkCopyTimeout = null, int batchSize = 5000, SqlBulkCopyOptions? copyOptions = null, IUnitOfWork unitOfWork = null);
        Task<int> BulkMergeAsync(IEnumerable<TEntity> items, int? bulkCopyTimeout = null, IUnitOfWork unitOfWork = null);
    }
}
