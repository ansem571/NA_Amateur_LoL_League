using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<LookupEntity>> GetLookupEntitiesByCategory(string category)
        {
            var entities = await _table.ReadManyAsync("Category = @category", new {category});
            return entities;
        }

        public async Task<LookupEntity> GetLookupByCategoryAndEnumAsync(string category, string enumValue)
        {
            var entity = (await _table.ReadManyAsync("Category = @category AND Enum = @enumValue", new {category, enumValue})).FirstOrDefault();
            return entity;
        }
    }
}
