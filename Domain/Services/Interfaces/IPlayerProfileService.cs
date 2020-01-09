using System;
using System.Threading.Tasks;
using Domain.Views;

namespace Domain.Services.Interfaces
{
    public interface IPlayerProfileService
    {
        Task<PlayerProfileView> GetPlayerProfileAsync(Guid userId);
    }
}
