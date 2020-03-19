using System;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using Domain.Season3Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Domain.Season3Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<UserRoleEntity> _userRoleManager;
        private readonly IBlacklistRepository _blacklistRepository;
        public UserService(UserManager<UserEntity> userManager, RoleManager<UserRoleEntity> userRoleManager, IBlacklistRepository blacklistRepository)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userRoleManager = userRoleManager ?? throw new ArgumentNullException(nameof(userRoleManager));
            _blacklistRepository = blacklistRepository ?? throw new ArgumentNullException(nameof(blacklistRepository));
        }

        public async Task<bool> BanUserAsync(Guid userId)
        {
            var blacklistEntity = new BlacklistEntity
            {
                Id = Guid.NewGuid(),
                IsBanned = true,
                UserId = userId
            };

            return await _blacklistRepository.CreateAsync(blacklistEntity);
        }

        public async Task<bool> UpdateBannedUserAsync(Guid userId)
        {
            var blacklistEntity = await _blacklistRepository.GetByUserIdAsync(userId);
            if (blacklistEntity == null)
            {
                return await BanUserAsync(userId);
            }
            blacklistEntity.IsBanned = !blacklistEntity.IsBanned;


            return await _blacklistRepository.UpdateAsync(blacklistEntity);
        }

        public async Task<bool> UpdateUserWithNewRoleAsync(Guid userId, RoleEnum role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (!await _userManager.IsInRoleAsync(user, role.ToString()))
            {
                await _userManager.AddToRoleAsync(user, role.ToString());
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveUserFromRoleAsync(Guid userId, RoleEnum role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (!await _userManager.IsInRoleAsync(user, role.ToString()))
            {
                await _userManager.RemoveFromRoleAsync(user, role.ToString());
                return true;
            }

            return false;
        }

        public async Task<bool> ValidateUserEmailAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (!user.ConfirmedEmail)
            {
                user.ConfirmedEmail = true;
                await _userManager.UpdateAsync(user);
            }

            return true;
        }
    }
}
