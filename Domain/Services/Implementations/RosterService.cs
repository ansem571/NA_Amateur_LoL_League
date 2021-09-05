using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Helpers;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Implementations;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class RosterService : IRosterService
    {
        private readonly ISummonerMapper _summonerMapper;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly ITeamPlayerRepository _teamPlayerRepository;
        private readonly ITeamRosterRepository _teamRosterRepository;
        private readonly ITeamCaptainRepository _teamCaptainRepository;
        private readonly ISeasonInfoRepository _seasonInfoRepository;
        private readonly IDivisionRepository _divisionRepository;
        private readonly IPlayerStatsRepository _playerStatsRepository;
        private readonly IPlayerStatsMapper _playerStatsMapper;
        private readonly IAlternateAccountRepository _alternateAccountRepository;
        private readonly IMatchDetailRepository _matchDetailRepository;
        private readonly ISummonerRoleMapper _roleMapper;
        private readonly IMatchMvpRepository _matchMvpRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleMapper _scheduleMapper;

        public RosterService(ISummonerMapper summonerMapper, ISummonerInfoRepository summonerInfoRepository,
            ITeamPlayerRepository teamPlayerRepository, ITeamRosterRepository teamRosterRepository,
            ITeamCaptainRepository teamCaptainRepository, ISeasonInfoRepository seasonInfoRepository,
            IDivisionRepository divisionRepository, IPlayerStatsRepository playerStatsRepository, IPlayerStatsMapper playerStatsMapper,
            IAlternateAccountRepository alternateAccountRepository, IMatchDetailRepository matchDetailRepository, ISummonerRoleMapper roleMapper,
            IMatchMvpRepository matchMvpRepository, IScheduleRepository scheduleRepository, IScheduleMapper scheduleMapper)
        {
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
            _playerStatsRepository = playerStatsRepository ??
                                     throw new ArgumentNullException(nameof(playerStatsRepository));
            _playerStatsMapper = playerStatsMapper ??
                                 throw new ArgumentNullException(nameof(playerStatsMapper));
            _alternateAccountRepository = alternateAccountRepository ??
                                          throw new ArgumentNullException(nameof(alternateAccountRepository));
            _matchDetailRepository = matchDetailRepository ??
                                    throw new ArgumentNullException(nameof(matchDetailRepository));
            _roleMapper = roleMapper ??
                          throw new ArgumentNullException(nameof(roleMapper));
            _matchMvpRepository = matchMvpRepository ??
                                  throw new ArgumentNullException(nameof(matchMvpRepository));
            _scheduleRepository = scheduleRepository ??
                                  throw new ArgumentNullException(nameof(scheduleRepository));
            _scheduleMapper = scheduleMapper ??
                              throw new ArgumentNullException(nameof(scheduleMapper));
        }

        public async Task<SeasonInfoView> GetSeasonInfoView()
        {
            var view = new SeasonInfoView();
            //TODO: Uncomment when testing
            //var seasonInfoTask = _seasonInfoRepository.GetAllSeasonsAsync();

            //var seasons = (await seasonInfoTask).OrderByDescending(x => x.SeasonStartDate).ToList();
            //var seasonInfo = seasons[1];

            //TODO: Uncomment when ready to push
            var seasonInfo = await _seasonInfoRepository.GetCurrentSeasonAsync();

            var rostersTask = GetAllRosters(seasonInfo);

            var divisions = (await _divisionRepository.GetAllForSeasonAsync(seasonInfo.Id)).ToList();
            try
            {
                var rosters = (await rostersTask).ToList();

                foreach (var rosterView in rosters)
                {
                    var division = divisions.First(x =>
                        x.LowerLimit <= rosterView.TeamTierScore && x.UpperLimit >= rosterView.TeamTierScore);
                    rosterView.Division = new DivisionView
                    {
                        DivisionName = division.Name,
                        DivisionMinScore = division.LowerLimit,
                        DivisionId = division.Id
                    };
                }

                view.Rosters = rosters.OrderByDescending(x => x.Division.DivisionMinScore).ToList();
            }
            catch (Exception)
            {
                //no rosters finalized yet
            }

            view.SeasonInfo = new SeasonInfoViewPartial
            {
                SeasonInfoId = seasonInfo.Id,
                ClosedRegistrationDate = seasonInfo.ClosedRegistrationDate,
                SeasonName = seasonInfo.SeasonName,
                SeasonStartDate = seasonInfo.SeasonStartDate
            };
            return view;
        }

        /// <summary>
        /// For Admin usage only
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<RosterView>> GetAllRosters(SeasonInfoEntity seasonInfo = null)
        {
            var list = new List<RosterView>();
            if (seasonInfo == null)
            {
                seasonInfo = await _seasonInfoRepository.GetCurrentSeasonAsync();
            }
            var rostersTask = _teamRosterRepository.GetAllTeamsAsync(seasonInfo.Id);
            var captainsTask = _teamCaptainRepository.GetAllTeamCaptainsAsync();
            var alternateAccountsTask = _alternateAccountRepository.ReadAllAsync();
            var allPlayersTask = _teamPlayerRepository.ReadAllForSeasonAsync(seasonInfo.Id);

            var rosters = await rostersTask;
            var captains = (await captainsTask).ToList();
            var alternateAccounts = (await alternateAccountsTask).ToList();
            var allPlayers = (await allPlayersTask).ToList();
            foreach (var roster in rosters)
            {
                var players = allPlayers.Where(x => x.TeamRosterId == roster.Id).ToList();
                var captain = captains.FirstOrDefault(x => x.TeamRosterId == roster.Id);

                var summoners =
                    (await _summonerInfoRepository.GetAllForSummonerIdsAsync(players.Select(x => x.SummonerId))).ToList();

                var alts = alternateAccounts.Where(x => summoners.Select(y => y.Id).ToList().Contains(x.SummonerId));
                var summonerViews = _summonerMapper.MapDetailed(summoners, alts, new List<PlayerStatsView>()).ToList();
                foreach (var teamPlayerEntity in players)
                {
                    var player = summoners.First(x => x.Id == teamPlayerEntity.SummonerId);

                    var isSub = teamPlayerEntity.IsSub ?? false;
                    var summonerView = summonerViews.First(x => x.SummonerName == player.SummonerName);
                    summonerView.IsSub = isSub;
                }

                var rosterView = new RosterView
                {
                    RosterId = roster.Id,
                    Captain = summoners.FirstOrDefault(x => x.Id == captain?.SummonerId)?.SummonerName,
                    TeamName = roster.TeamName,
                    Wins = roster.Wins ?? 0,
                    Loses = roster.Loses ?? 0,
                    Points = roster.Points ?? 0,
                    Players = summonerViews,
                    TeamTierScore = roster.TeamTierScore.GetValueOrDefault()
                };

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/logos", roster.Id.ToString());
                if (File.Exists(path))
                {
                    var byteData = await File.ReadAllBytesAsync(path);
                    var base64 = Convert.ToBase64String(byteData);
                    var type = GetContentType(path);
                    var imgSrc = String.Format($"data:{type};base64,{base64}");
                    rosterView.FileSource = imgSrc;
                }
                rosterView.Cleanup();
                list.Add(rosterView);
            }

            return list;
        }


        /// <summary>
        /// For Roster specific View
        /// </summary>
        /// <param name="rosterId"></param>
        /// <returns></returns>
        public async Task<RosterView> GetRosterAsync(Guid rosterId)
        {
            var seasonInfo = await _seasonInfoRepository.GetCurrentSeasonAsync();

            var alternateAccountsTask = _alternateAccountRepository.ReadAllAsync();
            var rosterTask = _teamRosterRepository.GetByTeamIdAsync(rosterId);
            var captainTask = _teamCaptainRepository.GetCaptainByRosterId(rosterId);
            var playersSummoner = (await _teamPlayerRepository.ReadAllForRosterAsync(rosterId)).ToList();
            var summonersTask = _summonerInfoRepository.GetAllForSummonerIdsAsync(playersSummoner.Select(x => x.SummonerId));
            var matchDetails = await _matchDetailRepository.GetMatchDetailsForPlayerAsync(playersSummoner.Select(x => x.SummonerId));
            var schedule = (await GetTeamSchedule(rosterId)).ToList();

            matchDetails = matchDetails.Where(x => x.Key.SeasonId == seasonInfo.Id).ToDictionary(x => x.Key, x => x.Value);
            var scheduleIds = schedule.Select(x => x.ScheduleId).ToList();

            var statIds = matchDetails.Values.SelectMany(x => x.Where(z => z.SeasonInfoId == seasonInfo.Id && scheduleIds.Contains(z.TeamScheduleId)).Select(y => y.PlayerStatsId));

            var mappedStats = await SetupPlayerStatsViews(statIds, schedule, matchDetails);

            var alternateAccounts = (await alternateAccountsTask).ToList();
            var summoners = (await summonersTask).ToList();

            var summonerViews = _summonerMapper.MapDetailed(summoners, alternateAccounts, mappedStats).ToList();

            var divisions = (await _divisionRepository.GetAllForSeasonAsync(seasonInfo.Id)).ToList();
            var captain = await captainTask;
            var roster = await rosterTask;
            foreach (var teamPlayerEntity in playersSummoner)
            {
                var player = summoners.First(x => x.Id == teamPlayerEntity.SummonerId);

                var isSub = teamPlayerEntity.IsSub ?? false;
                var summonerView = summonerViews.First(x => x.SummonerName == player.SummonerName);
                summonerView.IsSub = isSub;
            }

            var division = divisions.First(x =>
                x.LowerLimit <= roster.TeamTierScore && x.UpperLimit >= roster.TeamTierScore);
            var rosterView = new RosterView
            {
                RosterId = roster.Id,
                Captain = summoners.FirstOrDefault(x => x.Id == captain?.SummonerId)?.SummonerName,
                TeamName = roster.TeamName,
                Wins = roster.Wins ?? 0,
                Loses = roster.Loses ?? 0,
                Players = summonerViews,
                TeamTierScore = roster.TeamTierScore.GetValueOrDefault(),
                Division = new DivisionView
                {
                    DivisionName = division.Name,
                    DivisionMinScore = division.LowerLimit,
                    DivisionId = division.Id
                },
                Schedule = schedule
            };

            var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\logos");
            var path = Directory.GetFiles(directory).FirstOrDefault(x => x.Contains(rosterId.ToString()));

            if (File.Exists(path))
            {
                var byteData = await File.ReadAllBytesAsync(path);
                var base64 = Convert.ToBase64String(byteData);
                var type = GetContentType(path);
                var imgSrc = string.Format($"data:{type};base64,{base64}");
                rosterView.FileSource = imgSrc;

            }
            rosterView.Cleanup();

            return rosterView;
        }

        public async Task<IEnumerable<ScheduleView>> GetTeamSchedule(Guid rosterId)
        {
            var views = new List<ScheduleView>();
            //TODO: Uncomment when testing
            //var seasons = (await _seasonInfoRepository.GetAllSeasonsAsync()).OrderByDescending(x => x.SeasonStartDate).ToList();
            //var seasonInfo = seasons[1];

            //TODO: Uncomment when ready to push
            var seasonInfo = await _seasonInfoRepository.GetCurrentSeasonAsync();

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

        private async Task<List<PlayerStatsView>> SetupPlayerStatsViews(IEnumerable<Guid> statIds, List<ScheduleView> schedule, Dictionary<StatsKey, List<MatchDetailEntity>> matchDetails)
        {
            var playerStats = await _playerStatsRepository.GetStatsAsync(statIds);
            var mappedStats = new List<PlayerStatsView>();

            var scheduleIds = schedule.Select(x => x.ScheduleId).ToList();
            var mvpStats = await _matchMvpRepository.ReadAllForTeamScheduleIds(scheduleIds);
            foreach (var playerStat in playerStats)
            {
                var playerId = playerStat.Key.SummonerId;
                var playerMvps =
                    (mvpStats.SelectMany(x => x.Value)
                    .Where(x => x.BlueMvp == playerId || x.RedMvp == playerId || 
                        x.HonoraryBlueOppMvp == playerId || x.HonoraryRedOppMvp == playerId))
                    .GroupBy(x => (x.TeamScheduleId, x.Game))
                    .ToDictionary(x => x.Key, x => x.FirstOrDefault());
                var mvpPoints = 0d;
                if (playerMvps.Any())
                {
                    var matches =
                        (matchDetails.SelectMany(x => x.Value).Where(x => x.PlayerId == playerId)).GroupBy(x => (x.TeamScheduleId, x.Game)).ToDictionary(
                            x => x.Key, x => x.FirstOrDefault());
                    mvpPoints = CalculateMvpPoints(playerMvps, matches, scheduleIds);
                }

                var mappedStat = _playerStatsMapper.MapForSeason(playerStat.Value, mvpPoints);
                mappedStats.Add(mappedStat);
            }

            return mappedStats;
        }

        private double CalculateMvpPoints(IReadOnlyDictionary<(Guid, int), MatchMvpEntity> playerMvps, IReadOnlyDictionary<(Guid, int), MatchDetailEntity> matches, IEnumerable<Guid> teamScheduleIds)
        {
            var mvpPoints = 0d;
            foreach (var teamScheduleId in teamScheduleIds)
            {
                for (int game = 1; game <= 2; game++)
                {
                    playerMvps.TryGetValue((teamScheduleId, game), out var mvpEntity);
                    matches.TryGetValue((teamScheduleId, game), out var matchEntity);

                    if (mvpEntity != null && matchEntity != null)
                    {
                        if (matchEntity.Winner)
                        {
                            mvpPoints += 1;
                        }
                        else
                        {
                            mvpPoints += 0.5d;
                        }
                    }
                }
            }
            return mvpPoints;
        }

        public async Task<(bool result, string message)> SaveFileAsync(IFormFile file, Guid rosterId)
        {
            if (file == null || file.Length == 0)
                return (false, "file not selected");

            var roster = await _teamRosterRepository.GetByTeamIdAsync(rosterId);
            var extension = "";
            try
            {
                extension = GetMimeTypes().FirstOrDefault(x => x.Value.Equals(file.ContentType)).Key;
            }
            catch (Exception e)
            {
                return (false, "This file extension is not supported");
            }

            var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\logos");
            var files = Directory.GetFiles(directory).Where(x => x.Contains(rosterId.ToString()));


            foreach (var fileName in files)
            {
                File.Delete(fileName);
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\logos", $"{roster.Id.ToString()}{extension}");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, "");
        }

        public async Task<bool> UpdateTeamNameAsync(string newTeamName, Guid rosterId)
        {
            var roster = await _teamRosterRepository.GetByTeamIdAsync(rosterId);
            roster.TeamName = newTeamName;
            var updateResult = await _teamRosterRepository.UpdateAsync(roster);
            return updateResult;
        }

        public async Task<bool> SetPlayerAsSubAsync(string summonerName, Guid rosterId)
        {
            var summoner = (await _summonerInfoRepository.GetAllForSummonerNamesAsync(new List<string> { summonerName })).FirstOrDefault();
            if (summoner == null)
            {
                throw new Exception($"{summonerName} was not found");
            }
            var player = await _teamPlayerRepository.GetBySummonerIdAsync(summoner.Id, rosterId);
            player.IsSub = true;

            var result = await _teamPlayerRepository.UpdateAsync(new List<TeamPlayerEntity>
            {
                player
            });

            return result;
        }

        public async Task<bool> AddToTeamScoreAsync(string teamName, int wins, int loses)
        {
            var seasonInfo = await _seasonInfoRepository.GetCurrentSeasonAsync();
            var roster = await _teamRosterRepository.GetByTeamNameAsync(teamName, seasonInfo.Id);
            if (roster.Wins == null)
            {
                roster.Wins = 0;
                roster.Loses = 0;
                roster.Points = 0;
            }
            roster.Wins += wins;
            roster.Loses += loses;

            roster.Points += (int) (wins * 1.5);
            var result = await _teamRosterRepository.UpdateAsync(roster);
            return result;
        }

        public async Task<bool> UpdateRosterLineupAsync(UpdateRosterLineupView view)
        {
            var summoners = await _summonerInfoRepository.GetAllForSummonerIdsAsync(view.Lineup.Keys);

            var updateList = new List<SummonerInfoEntity>();
            foreach (var summoner in summoners)
            {
                var summonerDbRole = _roleMapper.Map(summoner.TeamRoleId.GetValueOrDefault());
                view.Lineup.TryGetValue(summoner.Id, out var viewSummoner);
                if (viewSummoner != null && viewSummoner.TeamRole != summonerDbRole)
                {
                    summoner.TeamRoleId = _roleMapper.Map(viewSummoner.TeamRole);
                    updateList.Add(summoner);
                }
            }

            return await _summonerInfoRepository.UpdateAsync(updateList);
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                { ".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"}
            };
        }
    }
}
