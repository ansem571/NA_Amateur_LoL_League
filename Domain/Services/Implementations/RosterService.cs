using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class RosterService : IRosterService
    {
        private readonly ILogger _logger;
        private readonly ISummonerMapper _summonerMapper;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly ITeamPlayerRepository _teamPlayerRepository;
        private readonly ITeamRosterRepository _teamRosterRepository;
        private readonly ITeamCaptainRepository _teamCaptainRepository;
        private readonly ISeasonInfoRepository _seasonInfoRepository;
        private readonly IDivisionRepository _divisionRepository;

        public RosterService(ILogger logger, ISummonerMapper summonerMapper, ISummonerInfoRepository summonerInfoRepository,
            ITeamPlayerRepository teamPlayerRepository, ITeamRosterRepository teamRosterRepository,
            ITeamCaptainRepository teamCaptainRepository, ISeasonInfoRepository seasonInfoRepository,
            IDivisionRepository divisionRepository)
        {
            _logger = logger ??
                      throw new ArgumentNullException(nameof(logger));
            _summonerMapper = summonerMapper ??
                              throw new ArgumentNullException(nameof(summonerMapper));
            _summonerInfoRepository = summonerInfoRepository ??
                                      throw new ArgumentNullException(nameof(summonerInfoRepository));
            _teamPlayerRepository = teamPlayerRepository ??
                                    throw new ArgumentNullException(nameof(teamPlayerRepository));
            _teamRosterRepository = teamRosterRepository ??
                                    throw new ArgumentNullException(nameof(teamRosterRepository));
            _teamCaptainRepository = teamCaptainRepository ??
                                     throw new ArgumentNullException(nameof(teamCaptainRepository));
            _seasonInfoRepository = seasonInfoRepository ??
                                    throw new ArgumentNullException(nameof(seasonInfoRepository));
            _divisionRepository = divisionRepository ??
                                  throw new ArgumentNullException(nameof(divisionRepository));
        }

        public async Task<SeasonInfoView> GetSeasonInfoView()
        {
            var view = new SeasonInfoView();

            var seasonInfoTask = _seasonInfoRepository.GetActiveSeasonInfoByDate(DateTime.Today);

            var seasonInfo = await seasonInfoTask;
            var divisions = (await _divisionRepository.GetAllForSeasonAsync(seasonInfo.Id)).ToList();
            try
            {
                var rostersTask = GetAllRosters();
                var rosters = (await rostersTask).ToList();

                foreach (var rosterView in rosters)
                {
                    rosterView.DivisionName = divisions.First(x =>
                        x.LowerLimit <= rosterView.TeamTierScore && x.UpperLimit >= rosterView.TeamTierScore).Name;
                }

                view.Rosters = rosters;
            }
            catch (Exception)
            {
                //no rosters finalized yet
            }

            view.SeasonInfo = new SeasonInfoViewPartial
            {
                ClosedRegistrationDate = seasonInfo.ClosedRegistrationDate,
                SeasonName = seasonInfo.SeasonName,
                SeasonStartDate = seasonInfo.SeasonStartDate
            };
            return view;
        }



        public async Task<IEnumerable<RosterView>> GetAllRosters()
        {
            var rosters = await _teamRosterRepository.GetAllTeamsAsync();
            var captains = (await _teamCaptainRepository.GetAllTeamCaptainsAsync()).ToList();
            var list = new List<RosterView>();
            foreach (var roster in rosters)
            {
                var players = await _teamPlayerRepository.ReadAllForRosterAsync(roster.Id);
                var captain = captains.FirstOrDefault(x => x.TeamRosterId == roster.Id);

                var summoners =
                    (await _summonerInfoRepository.GetAllForSummonerIdsAsync(players.Select(x => x.SummonerId))).ToList();

                var summonerViews = _summonerMapper.Map(summoners);
                var rosterView = new RosterView
                {
                    Captain = summoners.FirstOrDefault(x => x.Id == captain?.SummonerId)?.SummonerName,
                    TeamName = roster.TeamName,
                    Wins = roster.Wins ?? 0,
                    Loses = roster.Loses ?? 0,
                    Players = summonerViews,
                    TeamTierScore = roster.TeamTierScore.GetValueOrDefault()
                };
                rosterView.Cleanup();
                list.Add(rosterView);
            }

            return list;
        }
    }
}
