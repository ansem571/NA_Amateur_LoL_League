using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Helpers;
using Domain.Repositories.Implementations;
using Domain.Repositories.Interfaces;
using Domain.Views;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class PlayoffService
    {
        private readonly ILogger _logger;
        private readonly ITeamRosterRepository _teamRosterRepository;
        private readonly ISeasonInfoRepository _seasonInfoRepository;
        private readonly IDivisionRepository _divisionRepository;
        private readonly IPlayoffSeedRepository _playoffSeedRepository;

        public PlayoffService(IPlayoffSeedRepository playoffSeedRepository, ITeamRosterRepository teamRosterRepository, ISeasonInfoRepository seasonInfoRepository, IDivisionRepository divisionRepository)
        {
            _playoffSeedRepository =
                playoffSeedRepository ?? throw new ArgumentNullException(nameof(playoffSeedRepository));
            _teamRosterRepository = teamRosterRepository;
            _seasonInfoRepository = seasonInfoRepository;
            _divisionRepository = divisionRepository;
        }

        public async Task<IEnumerable<PlayoffSeedView>> GetPlayoffSchedule()
        {
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDateAsync(TimeZoneExtensions.GetCurrentTime().Date);

            var rostersTask = _teamRosterRepository.GetAllTeamsAsync(seasonInfo.Id);
            var divisions = _divisionRepository.GetAllForSeasonAsync(seasonInfo.Id);


            return null;
        }

        public async Task<IEnumerable<PlayoffSeedView>> GetView()
        {
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDateAsync(TimeZoneExtensions.GetCurrentTime().Date);
            var rostersTask = _teamRosterRepository.GetAllTeamsAsync(seasonInfo.Id);
            var divisions = _divisionRepository.GetAllForSeasonAsync(seasonInfo.Id);
            throw new NotImplementedException();
        }

        public async Task<bool> SetupPlayoffSchedule(IEnumerable<PlayoffSeedInsertView> playoffSeeds)
        {
            playoffSeeds = playoffSeeds.ToList();
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDateAsync(TimeZoneExtensions.GetCurrentTime().Date);

            var list = playoffSeeds.Select(playoffSeed => new PlayoffSeedEntity
            {
                Id = Guid.NewGuid(),
                RosterId = playoffSeed.RosterId,
                DivisionId = playoffSeed.DivisionId,
                SeasonInfoId = seasonInfo.Id,
                Seed = playoffSeed.Seed,
                PlayoffBracket = (int)playoffSeed.BracketFormat
            }).ToList();

            return await _playoffSeedRepository.InsertAsync(list);
        }
    }
}
