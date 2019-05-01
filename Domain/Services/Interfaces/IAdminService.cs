using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Views;

namespace Domain.Services.Interfaces
{
    public interface IAdminService
    {
        Task<SummonerTeamCreationView> GetSummonersToCreateTeamAsync();
        Task<IEnumerable<RosterView>> GetAllRosters();

        Task<bool> CreateNewTeamAsync(IEnumerable<Guid> summonerNames);
        Task<bool> RemovePlayerFromRosterAsync(Guid summonerId, Guid rosterId);
        Task<bool> AssignTeamCaptain(TeamCaptainView view);
    }
}
