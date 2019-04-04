using System;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.UserData;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class LookupRepository : ILookupRepository
    {
        private readonly ITableStorageRepository<LookupEntity> _table;

        public LookupRepository(ITableStorageRepository<LookupEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<LookupEntity> GetLookupEntity(Guid id)
        {
            var entity = await _table.ReadOneAsync(id);
            return entity;
        }
    }
}
