using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface IAchievementRepository
    {
        Task<IEnumerable<AchievementEntity>> GetAchievementsForUserAsync(Guid userId);
        Task<bool> InsertAsync(IEnumerable<AchievementEntity> entities);
    }
}
