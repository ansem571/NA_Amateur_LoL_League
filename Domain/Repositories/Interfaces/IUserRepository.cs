using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.UserData;

namespace Domain.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> InsertAsync(UserEntity user);
        Task<UserEntity> ReadOneAsync(Guid id);
        Task<UserEntity> ReadOneAsync(string email);
    }
}
