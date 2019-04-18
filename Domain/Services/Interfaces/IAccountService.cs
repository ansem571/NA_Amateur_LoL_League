using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Views;

namespace Domain.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> CreateSummonerInfoAsync(SummonerInfoView view, UserEntity user);
        Task<bool> UpdateSummonerInfoAsync(SummonerInfoView view, UserEntity user);

        Task<SummonerInfoView> GetSummonerViewAsync(UserEntity user);
        Task<IEnumerable<SummonerInfoView>> GetRosterSummonerInfosAsync(Guid rosterId);
        Task<IEnumerable<SummonerInfoView>> GetAllSummonersAsync();

        Task<SummonerRequestView> GetRequestedSummonersAsync(UserEntity user);
        Task<bool> UpdateSummonerRequestsAsync(UserEntity user, SummonerRequestView view);

        Task<List<RequestedPlayersView>> GetRequestedPlayersAsync();
        Task<FpSummonerView> GetFpSummonerView();
    }
}
