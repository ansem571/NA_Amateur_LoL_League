using System;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Season3Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> BanUserAsync(Guid userId);
        Task<bool> UpdateBannedUserAsync(Guid userId);
        Task<bool> UpdateUserWithNewRoleAsync(Guid userId, RoleEnum role);
        Task<bool> RemoveUserFromRoleAsync(Guid userId, RoleEnum role);
        Task<bool> ValidateUserEmailAsync(Guid userId);
    }
}
