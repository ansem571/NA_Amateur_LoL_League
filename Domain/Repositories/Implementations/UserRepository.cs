using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.UserData;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ITableStorageRepository<UserEntity> _table;

        public UserRepository(ITableStorageRepository<UserEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<bool> InsertAsync(UserEntity user)
        {
            return await _table.InsertAsync(user) == 1;
        }

        public async Task<UserEntity> ReadOneAsync(Guid id)
        {
            return await _table.ReadOneAsync(id);
        }

        public async Task<UserEntity> ReadOneAsync(string email)
        {
            var entity = (await _table.ReadManyAsync("Email = @email", new {email}, top: 1)).FirstOrDefault();
            return entity;
        }
    }
}
