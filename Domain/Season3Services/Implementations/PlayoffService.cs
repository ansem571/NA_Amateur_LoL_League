using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Helpers;
using Domain.Repositories.Implementations;
using Domain.Repositories.Interfaces;
using Domain.Season3Services.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.Extensions.Logging;

namespace Domain.Season3Services.Implementations
{
    public class PlayoffService : IPlayoffService
    {
        private readonly ILogger _logger;
        private readonly ITeamRosterRepository _teamRosterRepository;
        private readonly ISeasonInfoRepository _seasonInfoRepository;
        private readonly IDivisionRepository _divisionRepository;
        private readonly IPlayoffSeedRepository _playoffSeedRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleService _scheduleService;

        public PlayoffService(IPlayoffSeedRepository playoffSeedRepository, ITeamRosterRepository teamRosterRepository, ISeasonInfoRepository seasonInfoRepository,
            IDivisionRepository divisionRepository, IScheduleRepository scheduleRepository, IScheduleService scheduleService, ILogger logger)
        {
            _playoffSeedRepository =
                playoffSeedRepository ?? throw new ArgumentNullException(nameof(playoffSeedRepository));
            _teamRosterRepository = teamRosterRepository ?? throw new ArgumentNullException(nameof(teamRosterRepository));
            _seasonInfoRepository = seasonInfoRepository ?? throw new ArgumentNullException(nameof(seasonInfoRepository));
            _divisionRepository = divisionRepository ?? throw new ArgumentNullException(nameof(divisionRepository));
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Dictionary<string, List<ScheduleView>>> GetPlayoffSchedule()
        {
            var seasonInfo = await _seasonInfoRepository.GetCurrentSeasonAsync();

            var rostersTask = _teamRosterRepository.GetAllTeamsAsync(seasonInfo.Id);
            var divisionsTask = _divisionRepository.GetAllForSeasonAsync(seasonInfo.Id);

            var schedulesTask = _scheduleRepository.GetAllPlayoffMatchesAsync(seasonInfo.Id);

            var rosters = (await rostersTask).ToDictionary(x => x.Id, x => x);
            var divisions = (await divisionsTask).ToList();
            var schedules = (await schedulesTask).ToList();
            var views = _scheduleService.SetupSchedule(rosters, divisions, schedules);

            return views;
        }

        public async Task<bool> SetupPlayoffSchedule(IEnumerable<PlayoffSeedInsertView> playoffSeeds, DateTime weekOf, PlayoffFormat bracketFormat)
        {
            playoffSeeds = playoffSeeds.OrderBy(x => x.Seed).ToList();
            var seasonInfo = await _seasonInfoRepository.GetCurrentSeasonAsync();

            var list = playoffSeeds.Select(playoffSeed => new PlayoffSeedEntity
            {
                Id = Guid.NewGuid(),
                RosterId = playoffSeed.RosterId,
                DivisionId = playoffSeed.DivisionId,
                SeasonInfoId = seasonInfo.Id,
                Seed = playoffSeed.Seed,
                PlayoffBracket = (int)bracketFormat
            }).ToList();

            var insertPlayoffSeeds = await _playoffSeedRepository.InsertAsync(list);

            if (!insertPlayoffSeeds)
            {
                var message = "Error setting up seeding";
                _logger.LogError(message);
                throw new Exception(message);
            }

            var schedules = new List<ScheduleEntity>();
            switch (bracketFormat)
            {
                case PlayoffFormat.Standard:
                    {
                        //half as many matches as there are seeds
                        var matches = playoffSeeds.Count() / 2;
                        for (var i = 0; i < matches; i++)
                        {
                            var higherSeed = list[i];
                            var lowerSeed = list[playoffSeeds.Count() - i - 1];

                            schedules.Add(new ScheduleEntity
                            {
                                Id = Guid.NewGuid(),
                                HomeRosterTeamId = higherSeed.RosterId,
                                AwayRosterTeamId = lowerSeed.RosterId,
                                IsPlayoffMatch = true,
                                SeasonInfoId = seasonInfo.Id,
                                MatchWeek = weekOf,
                                AwayTeamWins = 0,
                                HomeTeamWins = 0
                            });
                        }

                    }
                    break;
                case PlayoffFormat.Gauntlet:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bracketFormat), bracketFormat, null);
            }

            return await _scheduleRepository.InsertAsync(schedules);
        }
    }
}
