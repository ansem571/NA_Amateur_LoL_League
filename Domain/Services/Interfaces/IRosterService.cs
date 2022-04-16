using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Views;
using Microsoft.AspNetCore.Http;

namespace Domain.Services.Interfaces
{
    public interface IRosterService
    {
        Task<SeasonInfoView> GetSeasonInfoView();
        Task<IEnumerable<RosterView>> GetAllRosters(SeasonInfoEntity seasonInfo = null);
        Task<RosterView> GetRosterAsync(Guid rosterId);
        Task<(bool result, string message)> SaveFileAsync(IFormFile file, Guid rosterId);
        Task<bool> UpdateTeamNameAsync(string newTeamName, Guid rosterId);
        Task<bool> SetPlayerAsSubAsync(string summonerName, Guid rosterId);
        Task<bool> AddToTeamScoreAsync(string teamName, int wins, int loses);

        Task<bool> UpdateRosterLineupAsync(UpdateRosterLineupView view);
        Task<IEnumerable<ScheduleView>> GetTeamSchedule(Guid rosterId);

        Task<bool> DeleteOldLogos(bool allLogos);
    }
}
