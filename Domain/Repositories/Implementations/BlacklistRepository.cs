using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.UserData;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class BlacklistRepository : IBlacklistRepository
    {
        private readonly ITableStorageRepository<BlacklistEntity> _table;

        public BlacklistRepository(ITableStorageRepository<BlacklistEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<BlacklistEntity> GetByUserIdAsync(Guid userId)
        {
            var result = (await _table.ReadManyAsync("UserId = @userId", new {userId}, top: 1)).FirstOrDefault();
            return result;
        }
    }
}
