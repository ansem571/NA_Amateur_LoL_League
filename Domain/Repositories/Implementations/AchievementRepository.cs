using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Forms;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class AchievementRepository : IAchievementRepository
    {
        private readonly ITableStorageRepository<AchievementEntity> _table;

        public AchievementRepository(ITableStorageRepository<AchievementEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<IEnumerable<AchievementEntity>> GetAchievementsForUserAsync(Guid userId)
        {
            return await _table.ReadManyAsync("UserId = @userId", new {userId});
        }

        public async Task<bool> InsertAsync(IEnumerable<AchievementEntity> entities)
        {
            entities = entities.ToList();
            if (entities.Any())
            {
                return await _table.InsertAsync(entities) == entities.Count();
            }

            return true;
        }
    }
}
