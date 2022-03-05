using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DAL.Contracts;
using DAL.Entities.LeagueInfo;
using Domain.Helpers;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly ILogger<AdminService> _logger;
        private readonly ILookupRepository _lookupRepository;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly ITeamPlayerRepository _teamPlayerRepository;
        private readonly ITeamRosterRepository _teamRosterRepository;
        private readonly ITeamCaptainRepository _teamCaptainRepository;
        private readonly IRosterService _rosterService;
        private readonly ITierDivisionMapper _tierDivisionMapper;
        private readonly IPlayerStatsRepository _playerStatsRepository;
        private readonly ISeasonInfoRepository _seasonInfoRepository;

        private const int MinTeamCountRequirement = 5;

        public AdminService(ILogger<AdminService> logger, ILookupRepository lookupRepository, ISummonerInfoRepository summonerInfoRepository,
            ITeamPlayerRepository teamPlayerRepository, ITeamRosterRepository teamRosterRepository,
            ITeamCaptainRepository teamCaptainRepository, IRosterService rosterService, ITierDivisionMapper tierDivisionMapper, IPlayerStatsRepository playerStatsRepository,
            ISeasonInfoRepository seasonInfoRepository)
        {
            _logger = logger ??
                      throw new ArgumentNullException(nameof(logger));
            _lookupRepository = lookupRepository ??
                                throw new ArgumentNullException(nameof(lookupRepository));
            _summonerInfoRepository = summonerInfoRepository ??
                                      throw new ArgumentNullException(nameof(summonerInfoRepository));
            _teamPlayerRepository = teamPlayerRepository ??
                                    throw new ArgumentNullException(nameof(teamPlayerRepository));
            _teamRosterRepository = teamRosterRepository ??
                                    throw new ArgumentNullException(nameof(teamRosterRepository));
            _teamCaptainRepository = teamCaptainRepository ??
                                     throw new ArgumentNullException(nameof(teamCaptainRepository));
            _rosterService = rosterService ??
                             throw new ArgumentNullException(nameof(rosterService));
            _tierDivisionMapper = tierDivisionMapper ??
                                  throw new ArgumentNullException(nameof(tierDivisionMapper));
            _playerStatsRepository = playerStatsRepository ??
                                     throw new ArgumentNullException(nameof(playerStatsRepository));
            _seasonInfoRepository = seasonInfoRepository ??
                                    throw new ArgumentNullException(nameof(playerStatsRepository));
        }

        public async Task<SummonerTeamCreationView> GetSummonersToCreateTeamAsync()
        {
            var seasonInfo = await _seasonInfoRepository.GetCurrentSeasonAsync();
            var summonersTask = _summonerInfoRepository.GetAllValidSummonersAsync();
            var rostersTask = _teamRosterRepository.GetAllTeamsAsync(seasonInfo.Id);

            var summoners = (await summonersTask).Where(x=>x.IsValidPlayer).ToList();
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
            return await _rosterService.GetAllRosters();
        }

        public async Task<bool> CreateNewTeamAsync(IEnumerable<Guid> summonerIds)
        {
            var seasonInfo = await _seasonInfoRepository.GetCurrentSeasonAsync();

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
                        },
                        seasonInfo.Id);

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


                var teamTierScores = new List<int>();
                var teamPlayers = new List<TeamPlayerEntity>();
                foreach (var summoner in summoners)
                {
                    teamPlayers.Add(new TeamPlayerEntity
                    {
                        SummonerId = summoner.Id,
                        SeasonInfoId = seasonInfo.Id
                    });
                    var currentTierScore = int.Parse((await _lookupRepository.GetLookupEntity(summoner.Tier_DivisionId)).Value);
                    var previousTierScore = summoner.PreviousSeasonRankId != null
                        ? int.Parse((await _lookupRepository.GetLookupEntity(summoner.Tier_DivisionId)).Value)
                        : 0;
                    var tierScore = Math.Max(currentTierScore, previousTierScore);
                    teamTierScores.Add(tierScore + summoner.CurrentLp);
                }

                var teamsCount = (await _teamRosterRepository.GetAllTeamsAsync(seasonInfo.Id)).Count();

                var teamTierScore = teamTierScores.OrderByDescending(x => x).Take(MinTeamCountRequirement).Sum();
                var team = new TeamRosterEntity
                {
                    Id = Guid.NewGuid(),
                    TeamName = $"Team{teamsCount+1}",
                    TeamTierScore = teamTierScore / MinTeamCountRequirement,
                    SeasonInfoId = seasonInfo.Id
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
            var seasonInfo = await _seasonInfoRepository.GetCurrentSeasonAsync();

            var summoner = (await _summonerInfoRepository.GetAllForSummonerNamesAsync(new List<string> { view.SummonerName })).FirstOrDefault();
            var roster = await _teamRosterRepository.GetByTeamNameAsync(view.RosterName.Trim(), seasonInfo.Id);

            if (summoner == null || roster == null)
            {
                return false;
            }
            
            var entity = new TeamCaptainEntity
            {
                SummonerId = summoner.Id,
                TeamRosterId = roster.Id
            };

            var captain = await _teamCaptainRepository.GetCaptainByRosterId(roster.Id);

            if (captain == null)
            {
                return await _teamCaptainRepository.CreateCaptainAsync(entity);
            }
            else
            {
                await _teamCaptainRepository.DeleteCaptainAsync(captain);
                return await _teamCaptainRepository.CreateCaptainAsync(entity);
            }
        }

        [Obsolete("Refer to interface")]
        public async Task<bool> UploadPlayerStatsAsync(IEnumerable<IFormFile> files)
        {
            var newStats = new List<PartialPlayerInfo>();
            var seasonInfo = await _seasonInfoRepository.GetCurrentSeasonAsync();
            var teamsTask = _teamRosterRepository.GetAllTeamsAsync(seasonInfo.Id);
            var registeredPlayersTask = _summonerInfoRepository.GetAllSummonersAsync();
            var playerStatsTask = _playerStatsRepository.GetAllStatsAsync(seasonInfo.Id);
            var teams = (await teamsTask).ToDictionary(x => x.TeamName, x => x);
            var teamPlayers = (await _teamPlayerRepository.ReadAllForSeasonAsync(seasonInfo.Id)).ToList();
            var registeredPlayers = (await registeredPlayersTask).ToDictionary(x => x.SummonerName.ToLowerInvariant(), x => x);

            foreach (var file in files)
            {
                if (file == null || file.Length <= 0)
                {
                    continue;
                }
                var split = file.FileName.Split("vs");
                var team1 = split[0].Trim();
                var regexSplit = Regex.Matches(split[1], @"^ [a-zA-z ]*");
                var team2 = regexSplit.First().Value;
                team2 = team2.Remove(team2.Length - 1).Trim();

                using (var stream = file.OpenReadStream())
                {
                    var matchStats = new List<PartialPlayerInfo>();
                    for (var i = 0; i < 10; i++)
                    {
                        matchStats.Add(new PartialPlayerInfo());
                    }

                    var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    var result = excelReader.AsDataSet();
                    for (var i = 0; i < result.Tables.Count; i++)
                    {
                        var sheet = result.Tables[i];
                        //Match tab
                        if (i == 0)
                        {
                            foreach (var stat in matchStats)
                            {
                                var strDuration = (string)sheet.Rows[1][4];
                                var duration = TimeSpan.Parse(strDuration);
                                stat.Duration = duration;
                            }
                        }
                        //Team tab
                        else if (i == 1)
                        {
                            //we skip this for now
                        }

                        //Player tab
                        else if (i == 2)
                        {
                            var totalTeamKillsBlue = 0;
                            var totalTeamKillsRed = 0;
                            for (var row = 1; row < sheet.Rows.Count; row++)
                            {
                                var player = matchStats[row - 1];
                                player.Name = (string)sheet.Rows[row][4];

                                //This is to check if a player is on the team from the match or an e-sub
                                registeredPlayers.TryGetValue(player.Name.ToLowerInvariant(), out var registeredPlayer);

                                teams.TryGetValue(team1, out var dbTeam1);
                                teams.TryGetValue(team2, out var dbTeam2);
                                if (registeredPlayer != null && (dbTeam1 != null || dbTeam2 != null))
                                {
                                    var teamPlayer = teamPlayers.FirstOrDefault(x =>
                                        x.SummonerId == registeredPlayer.Id && (x.TeamRosterId == dbTeam1?.Id || x.TeamRosterId == dbTeam2?.Id));
                                    if (teamPlayer == null)
                                    {
                                        player.SoftDelete = true;
                                    }
                                }

                                player.Assists = Convert.ToInt32((double)sheet.Rows[row][7]);
                                player.Deaths = Convert.ToInt32((double)sheet.Rows[row][9]);
                                player.Gold = Convert.ToInt32((double)sheet.Rows[row][16]);
                                player.Kills = Convert.ToInt32((double)sheet.Rows[row][20]);
                                if (row < 6)
                                {
                                    totalTeamKillsBlue += player.Kills;
                                }
                                else
                                {
                                    totalTeamKillsRed += player.Kills;
                                }
                                player.CS = Convert.ToInt32((double)sheet.Rows[row][22]);
                                player.VisionScore = Convert.ToInt32((double)sheet.Rows[row][23]);
                            }
                            for (var j = 0; j < matchStats.Count; j++)
                            {
                                var player = matchStats[j];
                                if (j < 5)
                                {
                                    player.TotalTeamKills = totalTeamKillsBlue;
                                }
                                else
                                {
                                    player.TotalTeamKills += totalTeamKillsRed;
                                }
                            }
                        }
                    }

                    newStats.AddRange(matchStats);
                    stream.Close();
                }
            }

            var newList = new List<PartialPlayerInfo>();
            foreach (var newStat in newStats)
            {
                if (newStat.SoftDelete)
                {
                    continue;
                }

                var existing = newList.FirstOrDefault(x => x.Name.ToLowerInvariant() == newStat.Name.ToLowerInvariant());
                if (existing != null)
                {
                    existing.AddOtherGame(newStat);
                }
                else
                {
                    newList.Add(newStat);
                }
            }
            var playerStats = (await playerStatsTask).ToDictionary(x => x.SummonerId, x => x);

            var updateList = new List<PlayerStatsEntity>();
            var insertList = new List<PlayerStatsEntity>();

            foreach (var playersStat in newList)
            {
                if (registeredPlayers.TryGetValue(playersStat.Name.ToLowerInvariant(), out var registeredPlayer))
                {
                    if (playerStats.TryGetValue(registeredPlayer.Id, out var oldStats))
                    {
                        oldStats.UpdateValues(playersStat);
                        updateList.Add(oldStats);
                    }
                    else
                    {
                        var newStat = new PlayerStatsEntity(playersStat, registeredPlayer.Id, seasonInfo.Id);
                        insertList.Add(newStat);
                    }
                }
            }

            var insertResult = _playerStatsRepository.InsertAsync(insertList);
            var updateResult = _playerStatsRepository.UpdateAsync(updateList);
            return await insertResult && await updateResult;
        }

        public async Task<bool> UpdateRosterTierScoreAsync()
        {
            var rosters = await _rosterService.GetAllRosters();

            foreach (var roster in rosters)
            {
                var teamTierScores = new List<int>();
                foreach (var player in roster.Players)
                {
                    var divisionId = _tierDivisionMapper.Map(player.TierDivision);
                    var divisionScore = int.Parse((await _lookupRepository.GetLookupEntity(divisionId)).Value);
                    teamTierScores.Add(divisionScore + player.CurrentLp);
                }
                var currentScore = teamTierScores.OrderByDescending(x => x).Take(MinTeamCountRequirement).Sum() / MinTeamCountRequirement;
                var rosterEntity = await _teamRosterRepository.GetByTeamIdAsync(roster.RosterId);
                rosterEntity.TeamTierScore = currentScore;
                try
                {
                    var rosterUpdate = await _teamRosterRepository.UpdateAsync(rosterEntity);
                    if (!rosterUpdate)
                    {
                        throw new Exception($"Error updating team score for: {roster.TeamName}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "");
                    return false;
                }
            }
            return true;
        }
    }
}
