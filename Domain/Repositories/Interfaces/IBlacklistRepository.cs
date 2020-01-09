using System;
using System.Threading.Tasks;
using DAL.Entities.UserData;

namespace Domain.Repositories.Interfaces
{
    public interface IBlacklistRepository
    {
        Task<BlacklistEntity> GetByUserIdAsync(Guid userId);
    }
}
