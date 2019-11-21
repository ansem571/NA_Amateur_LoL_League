using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Views;
using Microsoft.AspNetCore.Http;

namespace Domain.Services.Interfaces
{
    public interface IAdminService
    {
        Task<SummonerTeamCreationView> GetSummonersToCreateTeamAsync();
        Task<IEnumerable<RosterView>> GetAllRosters();
        Task<bool> UpdateRosterTierScoreAsync();

        Task<bool> CreateNewTeamAsync(IEnumerable<Guid> summonerNames);
        Task<bool> RemovePlayerFromRosterAsync(Guid summonerId, Guid rosterId);
        Task<bool> AssignTeamCaptain(TeamCaptainView view);

        [Obsolete("We will be using a new service in this ones place", true)]
        Task<bool> UploadPlayerStatsAsync(IEnumerable<IFormFile> files);
    }
}
