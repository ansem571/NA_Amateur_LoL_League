﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Views;

namespace Domain.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<Dictionary<string, List<ScheduleView>>> GetAllSchedules();
        Task<bool> UpdateScheduleAsync(ScheduleView view);
        Task<bool> CreateFullScheduleAsync();
        Task<Dictionary<string, IEnumerable<RosterView>>> SetupStandings();
        Task<Guid> GetDivisionIdByScheduleAsync(Guid scheduleId);

        Dictionary<string, List<ScheduleView>> SetupSchedule(Dictionary<Guid, TeamRosterEntity> rosters,
            List<DivisionEntity> divisions, List<ScheduleEntity> schedules);
    }
}
