using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Views;
using Microsoft.AspNetCore.Http;

namespace Domain.Services.Interfaces
{
    public interface IRosterService
    {
        Task<SeasonInfoView> GetSeasonInfoView();
        Task<IEnumerable<RosterView>> GetAllRosters();
        Task<RosterView> GetRosterAsync(Guid rosterId);
        Task<(bool result, string message)> SaveFileAsync(IFormFile file, Guid rosterId);
        Task<bool> UpdateTeamNameAsync(string newTeamName, Guid rosterId);
        Task<bool> SetPlayerAsSubAsync(string summonerName, Guid rosterId);
    }
}
