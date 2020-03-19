using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.Views;

namespace Domain.Season3Services.Interfaces
{
    public interface IPlayoffService
    {
        Task<Dictionary<string, List<ScheduleView>>> GetPlayoffSchedule();
        Task<bool> SetupPlayoffSchedule(IEnumerable<PlayoffSeedInsertView> playoffSeeds, DateTime weekOf, PlayoffFormat bracketFormat);
    }
}
