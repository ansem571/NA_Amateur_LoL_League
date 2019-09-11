using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface IMatchDetailsService
    {
        Task<bool> SendMatchDetailAsync(string matchDetailId);
    }
}
