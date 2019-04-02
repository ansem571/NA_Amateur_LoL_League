using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;

namespace Domain.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public AccountService(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        }

        public async Task<bool> RegisterUser(string email, string passWord, string confirmPassword)
        {
            var existingEntity = await _userRepository.ReadOneAsync(email);
            if (existingEntity != null || passWord != confirmPassword)
            {
                return false;
            }

            var encryptedPass = _passwordService.Encrypt("Casual eSports Amateur League", passWord);
            var entity = new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = encryptedPass,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                UserName = email
            };
            return await _userRepository.InsertAsync(entity);
        }

        public async Task<UserEntity> LoginAsync(string email, string passWord)
        {
            var user = await _userRepository.ReadOneAsync(email);
            if (user == null)
            {
                return null;
            }

            var decryptedPass = _passwordService.Decrypt(user.PasswordHash, passWord);
            return decryptedPass == passWord ? user : null;
        }
    }
}
