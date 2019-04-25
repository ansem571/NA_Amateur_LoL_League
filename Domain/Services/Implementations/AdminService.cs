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
    public class AdminService : IAdminService
    {
        private readonly ILogger _logger;
        private readonly ISummonerMapper _summonerMapper;
        private readonly IAlternateAccountMapper _alternateAccountMapper;
        private readonly ILookupRepository _lookupRepository;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly IAlternateAccountRepository _alternateAccountRepository;
        private readonly IRequestedSummonerRepository _requestedSummonerRepository;
        private readonly ITeamPlayerRepository _teamPlayerRepository;
        private readonly ITeamRosterRepository _teamRosterRepository;
        private readonly ITeamCaptainRepository _teamCaptainRepository;

        private const int MinTeamCountRequirement = 5;

        public AdminService(ILogger logger, ISummonerMapper summonerMapper, IAlternateAccountMapper alternateAccountMapper,
            ILookupRepository lookupRepository, ISummonerInfoRepository summonerInfoRepository,
            IAlternateAccountRepository alternateAccountRepository, IRequestedSummonerRepository requestedSummonerRepository,
            ITeamPlayerRepository teamPlayerRepository, ITeamRosterRepository teamRosterRepository,
            ITeamCaptainRepository teamCaptainRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _summonerMapper = summonerMapper ?? throw new ArgumentNullException(nameof(summonerMapper));
            _alternateAccountMapper = alternateAccountMapper ??
                                      throw new ArgumentNullException(nameof(alternateAccountMapper));

            _lookupRepository = lookupRepository ?? throw new ArgumentNullException(nameof(lookupRepository));
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
            _alternateAccountRepository = alternateAccountRepository ??
                                          throw new ArgumentNullException(nameof(alternateAccountRepository));
            _requestedSummonerRepository = requestedSummonerRepository ??
                                           throw new ArgumentNullException(nameof(requestedSummonerRepository));
            _teamPlayerRepository = teamPlayerRepository ??
                                    throw new ArgumentNullException(nameof(teamPlayerRepository));
            _teamRosterRepository = teamRosterRepository ??
                                    throw new ArgumentNullException(nameof(teamRosterRepository));
            _teamCaptainRepository = teamCaptainRepository ??
                                     throw new ArgumentNullException(nameof(teamCaptainRepository));
        }

        public async Task<SummonerTeamCreationView> GetSummonersToCreateTeamAsync()
        {
            var summonersTask = _summonerInfoRepository.GetAllValidSummonersAsync();
            var rostersTask = _teamRosterRepository.GetAllTeamsAsync();

            var summoners = (await summonersTask).ToList();
            var rosters = await rostersTask;

            var view = new SummonerTeamCreationView
            {
                SummonerInfos = new List<SummonerInfo>()
            };
            var unassignedPlayers = summoners.ToList();

            foreach (var roster in rosters)
            {
                var summonerIds = (await _teamPlayerRepository.ReadAllForRosterAsync(roster.Id)).Select(x => x.SummonerId);
                foreach (var summonerId in summonerIds)
                {
                    var summoner = summoners.FirstOrDefault(x => x.Id == summonerId);
                    if (summoner != null)
                    {
                        unassignedPlayers.Remove(summoner);
                    }
                }
            }

            foreach (var unassignedPlayer in unassignedPlayers)
            {
                var info = new SummonerInfo
                {
                    SummonerId = unassignedPlayer.Id,
                    SummonerName = unassignedPlayer.SummonerName
                };
                view.SummonerInfos.Add(info);
            }
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
                    Captain = summoners.FirstOrDefault(x=>x.Id == captain?.SummonerId)?.SummonerName,
                    TeamName = roster.TeamName,
                    Wins = roster.Wins ?? 0,
                    Loses = roster.Loses ?? 0,
                    Players = summonerViews
                };
                list.Add(rosterView);
            }

            return list;
        }

        public async Task<bool> CreateNewTeamAsync(IEnumerable<Guid> summonerIds)
        {
            summonerIds = summonerIds.ToList();
            var result = false;
            try
            {
                var summonerIdsList = summonerIds.ToList();
                var temp = new List<Guid>(summonerIds);
                foreach (var summonerId in temp)
                {
                    var playerRecord = await _teamPlayerRepository.GetRosterIdForExistingGroupAsync(
                        new List<Guid>
                        {
                            summonerId
                        });

                    if (playerRecord != null)
                    {
                        summonerIdsList.Remove(summonerId);
                    }
                }
                var summoners = (await _summonerInfoRepository.GetAllForSummonerIdsAsync(summonerIdsList)).ToList();



                if (summoners.Count < MinTeamCountRequirement)
                {
                    throw new Exception("Cannot make a team with less than 5 players");
                }

                var teamTierScore = 0;
                var teamPlayers = new List<TeamPlayerEntity>();
                foreach (var summoner in summoners)
                {
                    teamPlayers.Add(new TeamPlayerEntity
                    {
                        SummonerId = summoner.Id
                    });
                    var tierScore = int.Parse((await _lookupRepository.GetLookupEntity(summoner.Tier_DivisionId)).Value);
                    teamTierScore += tierScore + summoner.CurrentLp;
                }

                var teamsCount = (await _teamRosterRepository.GetAllTeamsAsync()).Count();
                var team = new TeamRosterEntity
                {
                    Id = Guid.NewGuid(),
                    TeamName = $"Team{teamsCount}",
                    TeamTierScore = teamTierScore / summoners.Count
                };

                var createTeamResult = await _teamRosterRepository.CreateAsync(team);
                if (!createTeamResult)
                {
                    throw new Exception("Failed to create team entity");
                }

                teamPlayers.ForEach(x => x.TeamRosterId = team.Id);

                result = await _teamPlayerRepository.InsertAsync(teamPlayers);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error for Admin Create Team process.");
            }

            return result;
        }

        public async Task<bool> RemovePlayerFromRosterAsync(Guid summonerId, Guid rosterId)
        {
            var result = false;
            try
            {
                var summoner = await _summonerInfoRepository.ReadOneBySummonerIdAsync(summonerId);

                var players = await _teamPlayerRepository.ReadAllForRosterAsync(rosterId);
                var player = players.FirstOrDefault(x => x.SummonerId == summoner.Id);
                if (player == null)
                {
                    throw new Exception($"Player: {summonerId} does not exist on roster id: {rosterId}");
                }

                result = await _teamPlayerRepository.DeleteAsync(new List<TeamPlayerEntity> { player });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error for Admin Remove Player from Team process.");
            }

            return result;
        }

        public async Task<bool> AssignTeamCaptain(TeamCaptainView view)
        {
            var summoner = (await _summonerInfoRepository.GetAllForSummonerNamesAsync(new List<string>{ view.SummonerName})).FirstOrDefault();
            var roster = await _teamRosterRepository.GetByTeamNameAsync(view.RosterName);

            if (summoner == null || roster == null)
            {
                return false;
            }

            var entity = new TeamCaptainEntity
            {
                SummonerId = summoner.Id,
                TeamRosterId = roster.Id
            };

            return await _teamCaptainRepository.CreateCaptainAsync(entity);
        }
    }
}
