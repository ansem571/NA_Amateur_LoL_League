using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.Views;

namespace Domain.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<Dictionary<string, List<ScheduleView>>> GetAllSchedules();
        Task<IEnumerable<ScheduleView>> GetTeamSchedule(Guid rosterId);
        Task<bool> UpdateScheduleAsync(ScheduleView view);
        Task<bool> CreateFullScheduleAsync();
        Task<Dictionary<string, IEnumerable<RosterView>>> SetupStandings();
    }
}
