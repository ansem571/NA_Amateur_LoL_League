using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.UserData;

namespace Domain.Repositories.Interfaces
{
    public interface IBlacklistRepository
    {
        Task<BlacklistEntity> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<BlacklistEntity>> GetAllAsync();
        Task<bool> CreateAsync(BlacklistEntity entity);
        Task<bool> UpdateAsync(BlacklistEntity entity);
    }
}
