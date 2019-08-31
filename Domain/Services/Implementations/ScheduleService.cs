using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Helpers;
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
        private readonly IRosterService _rosterService;

        public ScheduleService(ILogger logger, ITeamRosterRepository teamRosterRepository, ISeasonInfoRepository seasonInfoRepository,
            IDivisionRepository divisionRepository, IScheduleRepository scheduleRepository, IScheduleMapper scheduleMapper,
            IRosterService rosterService)
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
            _rosterService = rosterService ?? throw new ArgumentNullException(nameof(rosterService));
        }

        public async Task<Dictionary<string, List<ScheduleView>>> GetAllSchedules()
        {
            var views = new Dictionary<string, List<ScheduleView>>();
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDate(TimeZoneExtensions.GetCurrentTime());
            var divisionsTask = _divisionRepository.GetAllForSeasonAsync(seasonInfo.Id);
            var schedulesTask = _scheduleRepository.GetAllAsync(seasonInfo.Id);
            var rostersTask = _teamRosterRepository.GetAllTeamsAsync(seasonInfo.Id);

            var rosters = (await rostersTask).ToDictionary(x => x.Id, x => x);
            var divisions = (await divisionsTask).ToList();
            var schedules = (await schedulesTask).ToList();
            foreach (var schedule in schedules)
            {
                rosters.TryGetValue(schedule.HomeRosterTeamId, out var homeTeam);
                rosters.TryGetValue(schedule.AwayRosterTeamId, out var awayTeam);

                var homeTeamName = homeTeam?.TeamName ?? "BYE";
                var awayTeamName = awayTeam?.TeamName ?? "BYE";
                if (homeTeamName == "BYE" || awayTeamName == "BYE")
                {
                    continue;
                }

                string division;
                if (homeTeam != null)
                {
                    division = divisions.First(x =>
                        x.LowerLimit <= homeTeam.TeamTierScore && x.UpperLimit >= homeTeam.TeamTierScore).Name;
                }
                else if (awayTeam != null)
                {
                    division = divisions.First(x =>
                        x.LowerLimit <= awayTeam.TeamTierScore && x.UpperLimit >= awayTeam.TeamTierScore).Name;
                }
                else
                {
                    continue;
                }

                views.TryGetValue(division, out var view);
                if (view == null)
                {
                    views.Add(division, new List<ScheduleView>
                    {
                        _scheduleMapper.Map(schedule, homeTeamName, awayTeamName)
                    });
                }
                else
                {
                    view.Add(_scheduleMapper.Map(schedule, homeTeamName, awayTeamName));
                }
            }

            views = views.OrderBy(x => x.Key).ThenBy(x => x.Value.Select(y => y.WeekOf)).ToDictionary(x => x.Key, x => x.Value);

            return views;
        }

        public async Task<IEnumerable<ScheduleView>> GetTeamSchedule(Guid rosterId)
        {
            var views = new List<ScheduleView>();
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDate(TimeZoneExtensions.GetCurrentTime().Date);
            var schedulesTask = _scheduleRepository.GetAllAsync(seasonInfo.Id);
            var rostersTask = _teamRosterRepository.GetAllTeamsAsync(seasonInfo.Id);

            var rosters = (await rostersTask).ToDictionary(x => x.Id, x => x);
            var schedules = (await schedulesTask).ToList();
            foreach (var schedule in schedules)
            {
                if (schedule.HomeRosterTeamId != rosterId && schedule.AwayRosterTeamId != rosterId)
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

            return views;
        }

        public async Task<bool> UpdateScheduleAsync(ScheduleView view)
        {
            var scheduleInfo = await _scheduleRepository.GetScheduleAsync(view.ScheduleId);
            if (view.CasterName != null)
            {
                scheduleInfo.CasterName = view.CasterName;
            }

            if (view.PlayTime != null)
            {
                scheduleInfo.MatchScheduledTime = view.PlayTime;
            }

            if (view.HomeTeamScore != -1)
            {
                scheduleInfo.HomeTeamWins = view.HomeTeamScore;
            }

            if (view.AwayTeamScore != -1)
            {
                scheduleInfo.AwayTeamWins = view.AwayTeamScore;
            }

            return await _scheduleRepository.UpdateAsync(new List<ScheduleEntity> { scheduleInfo });
        }

        //TODO
        public async Task<bool> CreateFullScheduleAsync()
        {
            try
            {
                var seasonInfo = await _rosterService.GetSeasonInfoView();
                var divisions = seasonInfo.Rosters.GroupBy(x => x.Division.DivisionName)
                    .ToDictionary(x => x.Key, x => x.ToList());
                var scheduleEntities = new List<ScheduleEntity>();
                foreach (var division in divisions)
                {
                    var teamNames = division.Value.Select(x => x.TeamName).ToList();
                    var views = GenerateRoundRobin(teamNames);
                    foreach (var view in views)
                    {
                        var homeRosterId = division.Value.FirstOrDefault(x => x.TeamName == view.HomeTeam)?.RosterId;
                        var awayRosterId = division.Value.FirstOrDefault(x => x.TeamName == view.AwayTeam)?.RosterId;
                        var swap = TrueRandom.TrueRandomGenerator();

                        if (!swap)
                        {
                            var temp = homeRosterId;
                            homeRosterId = awayRosterId;
                            awayRosterId = temp;
                        }

                        var mapped = _scheduleMapper.Map(view, seasonInfo.SeasonInfo.SeasonInfoId, homeRosterId ?? Guid.Empty, awayRosterId ?? Guid.Empty);
                        scheduleEntities.Add(mapped);
                    }
                }
                return await _scheduleRepository.InsertAsync(scheduleEntities);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating schedule");
                return false;
            }
        }

        public async Task<Dictionary<string, IEnumerable<RosterView>>> SetupStandings()
        {
            var standings = new Dictionary<string, IEnumerable<RosterView>>();
            var schedulesTask = _scheduleRepository.GetAllUpdatedMatchesAsync();
            var rostersTask = _rosterService.GetSeasonInfoView();

            var rostersGrouped = (await rostersTask).Rosters.GroupBy(x => x.Division.DivisionName)
                .ToDictionary(x => x.Key, x => x.ToList());
            var schedules = (await schedulesTask).ToList();

            foreach (var division in rostersGrouped)
            {
                var teamsInDivision = division.Value.OrderByDescending(x => x.Points).ThenByDescending(x => x.Wins).ThenBy(x => x.Loses).ToList();
                var tempList = new List<RosterView>(teamsInDivision);

                foreach (var team in tempList)
                {
                    var sameScoreTeam = teamsInDivision.FirstOrDefault(x =>
                        x.RosterId != team.RosterId && x.Wins == team.Wins && x.Points == team.Points);
                    if (sameScoreTeam != null)
                    {
                        var schedule = schedules.FirstOrDefault(
                            x => (x.HomeRosterTeamId == team.RosterId && x.AwayRosterTeamId == sameScoreTeam.RosterId) ||
                            (x.AwayRosterTeamId == team.RosterId && x.HomeRosterTeamId == sameScoreTeam.RosterId));
                        if (schedule != null)
                        {
                            var teamPoints = schedule.HomeRosterTeamId == team.RosterId
                                ? schedule.HomeTeamWins
                                : schedule.AwayTeamWins;

                            var otherTeamPoints = schedule.HomeRosterTeamId == sameScoreTeam.RosterId
                                ? schedule.HomeTeamWins
                                : schedule.AwayTeamWins;

                            var team1Position = teamsInDivision.IndexOf(team);
                            var team2Position = teamsInDivision.IndexOf(sameScoreTeam);

                            if (teamPoints > otherTeamPoints && team1Position < team2Position)
                            {
                                teamsInDivision.RemoveAt(team2Position);
                                teamsInDivision.Insert(team1Position, sameScoreTeam);
                            }
                            else if (teamPoints < otherTeamPoints && team1Position > team2Position)
                            {
                                teamsInDivision.RemoveAt(team1Position);
                                teamsInDivision.Insert(team2Position, team);
                            }
                        }
                    }
                }

                standings.Add(division.Key, teamsInDivision);
            }

            return standings;
        }

        private IEnumerable<ScheduleView> GenerateRoundRobin(List<string> teamNames)
        {
            var views = new List<ScheduleView>();
            if (teamNames.Count == 7)
            {
                return GenerateSpecialRoundRobin(teamNames);
            }

            var teamSize = teamNames.Count;

            var weeks = Math.Min(teamSize - 1, 6);
            var halfSize = teamSize / 2;
            var teams = new List<string>();
            teams.AddRange(teamNames);
            teams.RemoveAt(0);

            teamSize--;

            var startDate = new DateTime(2019, 9, 4);
            for (var week = 0; week < weeks; week++)
            {
                var date = startDate.AddDays(7 * week);
                var teamIndex = week % teamSize;
                var firstTeam = teams[teamIndex];
                var secondTeam = teamNames[0];

                views.Add(new ScheduleView
                {
                    ScheduleId = Guid.NewGuid(),
                    HomeTeam = firstTeam,
                    AwayTeam = secondTeam,
                    HomeTeamScore = 0,
                    AwayTeamScore = 0,
                    WeekOf = date
                });

                for (var index = 1; index < halfSize; index++)
                {
                    firstTeam = teams[(week + index) % teamSize];
                    secondTeam = teams[(week + teamSize - index) % teamSize];
                    views.Add(new ScheduleView
                    {
                        ScheduleId = Guid.NewGuid(),
                        HomeTeam = firstTeam,
                        AwayTeam = secondTeam,
                        HomeTeamScore = 0,
                        AwayTeamScore = 0,
                        WeekOf = date
                    });
                }
            }

            return views;
        }

        private IEnumerable<ScheduleView> GenerateSpecialRoundRobin(List<string> teamNames)
        {
            var views = new List<ScheduleView>();

            const int weeks = 5;

            var startDate = new DateTime(2019, 9, 4);
            for (var week = 0; week < weeks; week++)
            {
                var date = startDate.AddDays(7 * week);
                views.AddRange(CreateForWeek(week, teamNames, date));
            }

            return views;
        }

        //TODO: Make a calculator for this, someday
        private IEnumerable<ScheduleView> CreateForWeek(int week, IEnumerable<string> teamNames, DateTime date)
        {
            var list = new List<ScheduleView>();
            var t = teamNames.ToList();
            //week 1
            if (week == 0)
            {
                list.Add(new ScheduleView(t[0], t[1], date));
                list.Add(new ScheduleView(t[2], t[3], date));
                list.Add(new ScheduleView(t[4], t[5], date));
                list.Add(new ScheduleView(t[0], t[6], date));
            }
            //week 2
            if (week == 1)
            {
                list.Add(new ScheduleView(t[0], t[2], date));
                list.Add(new ScheduleView(t[1], t[3], date));
                list.Add(new ScheduleView(t[4], t[6], date));
                list.Add(new ScheduleView(t[1], t[5], date));
            }
            //week 3
            if (week == 2)
            {
                list.Add(new ScheduleView(t[0], t[3], date));
                list.Add(new ScheduleView(t[1], t[4], date));
                list.Add(new ScheduleView(t[2], t[5], date));
                list.Add(new ScheduleView(t[3], t[6], date));
            }
            //week 4
            if (week == 3)
            {
                list.Add(new ScheduleView(t[0], t[4], date));
                list.Add(new ScheduleView(t[1], t[6], date));
                list.Add(new ScheduleView(t[3], t[5], date));
                list.Add(new ScheduleView(t[2], t[6], date));
            }
            //week 5
            if (week == 4)
            {
                list.Add(new ScheduleView(t[0], t[5], date));
                list.Add(new ScheduleView(t[1], t[2], date));
                list.Add(new ScheduleView(t[2], t[4], date));
                list.Add(new ScheduleView(t[3], t[4], date));
                list.Add(new ScheduleView(t[5], t[6], date));
            }

            return list;
        }
    }
}
