using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.AspNetCore.Http;
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
        private readonly IPlayerStatsRepository _playerStatsRepository;
        private readonly IPlayerStatsMapper _playerStatsMapper;
        private readonly IAlternateAccountRepository _alternateAccountRepository;

        public RosterService(ILogger logger, ISummonerMapper summonerMapper, ISummonerInfoRepository summonerInfoRepository,
            ITeamPlayerRepository teamPlayerRepository, ITeamRosterRepository teamRosterRepository,
            ITeamCaptainRepository teamCaptainRepository, ISeasonInfoRepository seasonInfoRepository,
            IDivisionRepository divisionRepository, IPlayerStatsRepository playerStatsRepository, IPlayerStatsMapper playerStatsMapper,
            IAlternateAccountRepository alternateAccountRepository)
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
            _playerStatsRepository = playerStatsRepository ??
                                     throw new ArgumentNullException(nameof(playerStatsRepository));
            _playerStatsMapper = playerStatsMapper ??
                                 throw new ArgumentNullException(nameof(playerStatsMapper));
            _alternateAccountRepository = alternateAccountRepository ??
                                          throw new ArgumentNullException(nameof(alternateAccountRepository));
        }

        public async Task<SeasonInfoView> GetSeasonInfoView()
        {
            var view = new SeasonInfoView();

            var rostersTask = GetAllRosters();
            var seasonInfoTask = _seasonInfoRepository.GetActiveSeasonInfoByDate(DateTime.Today);

            var seasonInfo = await seasonInfoTask;
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
                        DivisionMinScore = division.LowerLimit
                    };
                }

                view.Rosters = rosters.OrderByDescending(x => x.Division.DivisionMinScore);
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
        public async Task<IEnumerable<RosterView>> GetAllRosters()
        {
            var list = new List<RosterView>();
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDate(DateTime.Today);
            var rostersTask = _teamRosterRepository.GetAllTeamsAsync(seasonInfo.Id);
            var captainsTask = _teamCaptainRepository.GetAllTeamCaptainsAsync();
            var alternateAccountsTask = _alternateAccountRepository.ReadAllAsync();
            var allPlayersTask = _teamPlayerRepository.ReadAllAsync();

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
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDate(DateTime.Today);

            var alternateAccountsTask = _alternateAccountRepository.ReadAllAsync();
            var rosterTask = _teamRosterRepository.GetByTeamIdAsync(rosterId);
            var captainTask = _teamCaptainRepository.GetCaptainByRosterId(rosterId);
            var playersSummoner = (await _teamPlayerRepository.ReadAllForRosterAsync(rosterId)).ToList();
            var summonersTask = _summonerInfoRepository.GetAllForSummonerIdsAsync(playersSummoner.Select(x => x.SummonerId));
            var playerStats = await _playerStatsRepository.GetStatsForSummonersAsync(playersSummoner.Select(x => x.SummonerId), seasonInfo.Id);

            var mappedStats = _playerStatsMapper.Map(playerStats).ToList();

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
                    DivisionMinScore = division.LowerLimit
                }
            };

            var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\logos");
            var path = Directory.GetFiles(directory).FirstOrDefault(x => x.Contains(rosterId.ToString()));

            if (File.Exists(path))
            {
                var byteData = await File.ReadAllBytesAsync(path);
                var base64 = Convert.ToBase64String(byteData);
                var type = GetContentType(path);
                var imgSrc = String.Format($"data:{type};base64,{base64}");
                rosterView.FileSource = imgSrc;

            }
            rosterView.Cleanup();

            return rosterView;
        }

        public async Task<(bool result, string message)> SaveFileAsync(IFormFile file, Guid rosterId)
        {
            if (file == null || file.Length == 0)
                return (false, "file not selected");

            var roster = await _teamRosterRepository.GetByTeamIdAsync(rosterId);
            var extension = GetMimeTypes().FirstOrDefault(x => x.Value.Equals(file.ContentType)).Key;

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
            var summoner = (await _summonerInfoRepository.GetAllForSummonerNamesAsync(new List<string> {summonerName})).FirstOrDefault();
            if (summoner == null)
            {
                throw new Exception($"{summonerName} was not found");
            }
            var player = await _teamPlayerRepository.GetBySummonerIdAsync(summoner.Id);
            player.IsSub = true;

            var result = await _teamPlayerRepository.UpdateAsync(new List<TeamPlayerEntity>
            {
                player
            });

            return result;
        }

        public async Task<bool> AddToTeamScoreAsync(string teamName, int wins, int loses)
        {
            var roster = await _teamRosterRepository.GetByTeamNameAsync(teamName);
            roster.Wins += wins;
            roster.Loses += loses;

            if (wins == 2 && loses == 0)
            {
                roster.Points += 3;
            }
            else if(wins == 1 && loses == 1)
            {
                roster.Points += 1;
            }
            var result = await _teamRosterRepository.UpdateAsync(roster);
            return result;
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
