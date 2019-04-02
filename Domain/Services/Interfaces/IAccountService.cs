using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.UserData;

namespace Domain.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> RegisterUser(string email, string passWord, string confirmPassword);
    }
}
