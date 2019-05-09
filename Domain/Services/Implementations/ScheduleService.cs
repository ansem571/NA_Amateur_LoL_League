using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly ILogger _logger;
        private readonly ITeamRosterRepository _teamRosterRepository;
        private readonly ISeasonInfoRepository _seasonInfoRepository;
        private readonly IDivisionRepository _divisionRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleMapper _scheduleMapper;

        public ScheduleService(ILogger logger, ITeamRosterRepository teamRosterRepository, ISeasonInfoRepository seasonInfoRepository,
            IDivisionRepository divisionRepository, IScheduleRepository scheduleRepository, IScheduleMapper scheduleMapper)
        {
            _logger = logger ??
                      throw new ArgumentNullException(nameof(logger));
            _teamRosterRepository = teamRosterRepository ??
                                    throw new ArgumentNullException(nameof(teamRosterRepository));
            _seasonInfoRepository = seasonInfoRepository ??
                                    throw new ArgumentNullException(nameof(seasonInfoRepository));
            _divisionRepository = divisionRepository ??
                                  throw new ArgumentNullException(nameof(divisionRepository));
            _scheduleRepository = scheduleRepository ??
                                  throw new ArgumentNullException(nameof(scheduleRepository));
            _scheduleMapper = scheduleMapper ??
                              throw new ArgumentNullException(nameof(scheduleMapper));
        }

        public async Task<Dictionary<string, List<ScheduleView>>> GetAllSchedules()
        {
            var views = new Dictionary<string, List<ScheduleView>>();
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDate(DateTime.Now);
            var divisionsTask = _divisionRepository.GetAllForSeasonAsync(seasonInfo.Id);
            var schedulesTask = _scheduleRepository.GetAllAsync();
            var rostersTask = _teamRosterRepository.GetAllTeamsAsync();

            var rosters = (await rostersTask).ToDictionary(x => x.Id, x => x);
            var divisions = (await divisionsTask).ToList();
            var schedules = (await schedulesTask).ToList();
            foreach (var schedule in schedules)
            {
                rosters.TryGetValue(schedule.HomeRosterTeamId, out var homeTeam);
                rosters.TryGetValue(schedule.AwayRosterTeamId, out var awayTeam);

                if (homeTeam != null && awayTeam != null)
                {
                    var division = divisions.First(x =>
                        x.LowerLimit <= homeTeam.TeamTierScore && x.UpperLimit >= homeTeam.TeamTierScore).Name;
                    views.TryGetValue(division, out var view);
                    if (view == null)
                    {
                        views.Add(division, new List<ScheduleView>
                        {
                            _scheduleMapper.Map(schedule, homeTeam.TeamName, awayTeam.TeamName)
                        });
                    }
                    else
                    {
                        view.Add(_scheduleMapper.Map(schedule, homeTeam.TeamName, awayTeam.TeamName));
                    }
                }
            }

            views = views.OrderBy(x=>x.Key).ThenBy(x => x.Value.Select(y=>y.WeekOf)).ToDictionary(x=> x.Key, x=> x.Value);

            return views;
        }

        public async Task<IEnumerable<ScheduleView>> GetTeamSchedule(Guid rosterId)
        {
            var views = new List<ScheduleView>();
            var schedulesTask = _scheduleRepository.GetAllAsync();
            var rostersTask = _teamRosterRepository.GetAllTeamsAsync();

            var rosters = (await rostersTask).ToDictionary(x => x.Id, x => x);
            var schedules = (await schedulesTask).ToList();
            foreach (var schedule in schedules)
            {
                if (schedule.HomeRosterTeamId != rosterId || schedule.AwayRosterTeamId != rosterId)
                {
                    continue;
                }

                rosters.TryGetValue(schedule.HomeRosterTeamId, out var homeTeam);
                rosters.TryGetValue(schedule.AwayRosterTeamId, out var awayTeam);

                if (homeTeam != null && awayTeam != null)
                {
                    views.Add(_scheduleMapper.Map(schedule, homeTeam.TeamName, awayTeam.TeamName));
                }
            }

            views = views.OrderBy(x => x.WeekOf.Date).ToList();

            return views;
        }

        public async Task<bool> UpdateScheduleAsync(ScheduleView view)
        {
            var seasonTask = _seasonInfoRepository.GetActiveSeasonInfoByDate(DateTime.Now);
            var homeTeamTask = _teamRosterRepository.GetByTeamNameAsync(view.HomeTeam);
            var awayTeamTask = _teamRosterRepository.GetByTeamNameAsync(view.AwayTeam);

            var seasonInfo = await seasonTask;
            var homeTeam = await homeTeamTask;
            var awayTeam = await awayTeamTask;

            var mapped = _scheduleMapper.Map(view, seasonInfo.Id, homeTeam.Id, awayTeam.Id);

            return await _scheduleRepository.UpdateAsync(new List<ScheduleEntity> {mapped});
        }

        //TODO
        public async Task<bool> CreateFullScheduleAsync()
        {
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDate(DateTime.Now);
            var divisionsTask = _divisionRepository.GetAllForSeasonAsync(seasonInfo.Id);
            var rostersTask = _teamRosterRepository.GetAllTeamsAsync();

            var divisions = (await divisionsTask).ToList();
            var rosters = (await rostersTask).ToDictionary(x => x.Id, x => x);

            throw new NotImplementedException();
        }
    }
}
