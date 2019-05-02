using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public RosterService(ILogger logger, ISummonerMapper summonerMapper, ISummonerInfoRepository summonerInfoRepository,
            ITeamPlayerRepository teamPlayerRepository, ITeamRosterRepository teamRosterRepository,
            ITeamCaptainRepository teamCaptainRepository, ISeasonInfoRepository seasonInfoRepository,
            IDivisionRepository divisionRepository, IPlayerStatsRepository playerStatsRepository, IPlayerStatsMapper playerStatsMapper)
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

        /// <summary>
        /// For Admin usage only
        /// </summary>
        /// <returns></returns>
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

                var summonerViews = _summonerMapper.MapDetailed(summoners, new List<PlayerStatsView>());
                var rosterView = new RosterView
                {
                    RosterId = roster.Id,
                    Captain = summoners.FirstOrDefault(x => x.Id == captain?.SummonerId)?.SummonerName,
                    TeamName = roster.TeamName,
                    Wins = roster.Wins ?? 0,
                    Loses = roster.Loses ?? 0,
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
            var seasonInfoTask = _seasonInfoRepository.GetActiveSeasonInfoByDate(DateTime.Today);
            var rosterTask = _teamRosterRepository.GetByTeamIdAsync(rosterId);
            var captainTask = _teamCaptainRepository.GetCaptainByRosterId(rosterId);
            var playersSummonerIds = (await _teamPlayerRepository.ReadAllForRosterAsync(rosterId)).Select(x=>x.SummonerId).ToList();
            var summoners = (await _summonerInfoRepository.GetAllForSummonerIdsAsync(playersSummonerIds)).ToList();
            var playerStats = await _playerStatsRepository.GetStatsForSummonersAsync(playersSummonerIds);
            var mappedStats = _playerStatsMapper.Map(playerStats).ToList();
            var summonerViews = _summonerMapper.MapDetailed(summoners, mappedStats).ToList();

            var seasonInfo = await seasonInfoTask;
            var divisions = (await _divisionRepository.GetAllForSeasonAsync(seasonInfo.Id)).ToList();
            var captain = await captainTask;
            var roster = await rosterTask;

            var rosterView = new RosterView
            {
                RosterId = roster.Id,
                Captain = summoners.FirstOrDefault(x => x.Id == captain?.SummonerId)?.SummonerName,
                TeamName = roster.TeamName,
                Wins = roster.Wins ?? 0,
                Loses = roster.Loses ?? 0,
                Players = summonerViews,
                TeamTierScore = roster.TeamTierScore.GetValueOrDefault(),
                DivisionName = divisions.First(x =>
                    x.LowerLimit <= roster.TeamTierScore && x.UpperLimit >= roster.TeamTierScore).Name
            };

            var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\logos");
            var path = Directory.GetFiles(directory).FirstOrDefault(x=>x.Contains(rosterId.ToString()));

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
