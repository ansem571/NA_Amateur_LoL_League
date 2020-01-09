﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using DAL.Entities.LeagueInfo;
using DAL.Entities.UserData;
using Domain.Helpers;
using Domain.Repositories.Implementations;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.Extensions.Logging;
using RiotSharp.Endpoints.MatchEndpoint;
using RiotSharp.Endpoints.StaticDataEndpoint.Champion;

namespace Domain.Services.Implementations
{
    public class MatchDetailService : IMatchDetailService
    {
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;
        private readonly ISeasonInfoRepository _seasonInfoRepository;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly IPlayerStatsRepository _playerStatsRepository;
        private readonly IMatchDetailRepository _matchDetailRepository;
        private readonly IMatchMvpRepository _matchMvpRepository;
        private readonly IChampionStatsRepository _championStatsRepository;
        private readonly IScheduleService _scheduleService;
        private readonly ITeamPlayerRepository _teamPlayerRepository;
        private readonly ITeamRosterRepository _teamRosterRepository;
        private readonly IAchievementRepository _achievementRepository;

        private readonly string _wwwRootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        public MatchDetailService(ILogger logger, IEmailService emailService, IPlayerStatsRepository playerStatsRepository, ISummonerInfoRepository summonerInfoRepository,
            ISeasonInfoRepository seasonInfoRepository, IMatchDetailRepository matchDetailRepository, IMatchMvpRepository matchMvpRepository,
            IChampionStatsRepository championStatsRepository, IScheduleService scheduleService, ITeamPlayerRepository teamPlayerRepository, ITeamRosterRepository teamRosterRepository, IAchievementRepository achievementRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _playerStatsRepository = playerStatsRepository ?? throw new ArgumentNullException(nameof(playerStatsRepository));
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
            _seasonInfoRepository = seasonInfoRepository ?? throw new ArgumentNullException(nameof(seasonInfoRepository));
            _matchDetailRepository = matchDetailRepository ?? throw new ArgumentNullException(nameof(matchDetailRepository));
            _matchMvpRepository = matchMvpRepository ?? throw new ArgumentNullException(nameof(matchMvpRepository));
            _championStatsRepository = championStatsRepository ?? throw new ArgumentNullException(nameof(championStatsRepository));
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _teamPlayerRepository = teamPlayerRepository ?? throw new ArgumentNullException(nameof(teamPlayerRepository));
            _teamRosterRepository = teamRosterRepository ?? throw new ArgumentNullException(nameof(teamRosterRepository));
            _achievementRepository = achievementRepository ?? throw new ArgumentNullException(nameof(achievementRepository));
        }

        public async Task<bool> SendFileData(MatchSubmissionView view)
        {
            var addMatchStats = await UpdateStatsAsync(view);
            if (!addMatchStats)
            {
                return false;
            }

            var csvDataFile = CreateCsvDataFile(view);

            await _emailService.SendEmailAsync("casualesportsamateurleague@gmail.com", "Match result for subject", view.FileName, new List<Attachment>
            {
                new Attachment(csvDataFile)
            });

            return true;
        }

        private async Task<bool> UpdateStatsAsync(MatchSubmissionView view)
        {
            var divisionTask = _scheduleService.GetDivisionIdByScheduleAsync(view.ScheduleId);
            var matchesTask = _matchDetailRepository.ReadForScheduleId(view.ScheduleId);
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDateAsync(TimeZoneExtensions.GetCurrentTime().Date);
            var registeredPlayersTask = _summonerInfoRepository.GetAllValidSummonersAsync();
            var previousListedMvpsTask = _matchMvpRepository.ReadAllForTeamScheduleId(view.ScheduleId);
            var registeredPlayers = (await registeredPlayersTask).ToDictionary(x => x.SummonerName.ToLowerInvariant(), x => x);
            var matchDictionary = (await matchesTask);
            var mvpDetails = (await previousListedMvpsTask).OrderBy(x => x.Game).ToDictionary(x => x.Game, x => x);

            var divisionId = await divisionTask;
            var insertMvpDetails = new List<MatchMvpEntity>();
            var updateMvpDetails = new List<MatchMvpEntity>();
            var gameNum = 0;
            //Will always insert new records and never update, we will delete any old records first
            var insertDetailsList = new List<MatchDetailEntity>();
            var insertStatsList = new List<PlayerStatsEntity>();
            var championDetails = new List<ChampionStatsEntity>();
            var insertAchievementsList = new List<AchievementEntity>();
            //List of Ids to hard delete

            foreach (var gameInfo in view.GameInfos)
            {
                gameNum++;

                if (gameInfo.HomeTeamForfeit || gameInfo.AwayTeamForfeit)
                {
                    continue;
                }

                //matchhistory.na.leagueoflegends.com/en/#match-details/NA1/{match id}/{dont care}?tab=overview
                var split = gameInfo.MatchHistoryLink.Split("/");
                //If an invalid match was submitted, will fail the entire process
                if (!uint.TryParse(split[6], out var matchId))
                {
                    return false;
                }

                var version = (await GlobalVariables.ChampsApi.Versions.GetAllAsync()).First();
                var championsTask = GlobalVariables.ChampsApi.Champions.GetAllAsync(version);
                var riotMatchTask = GlobalVariables.Api.Match.GetMatchAsync(RiotSharp.Misc.Region.Na, matchId);

                var riotMatch = await riotMatchTask;
                var champions = await championsTask;

                CollectBans(view, riotMatch, champions, seasonInfo, divisionId, championDetails);

                var gameDuration = riotMatch.GameDuration;

                var matchList = new List<MatchDetailContract>();

                await CollectPlayerMatchDetailsAsync(view, riotMatch, champions, gameInfo, registeredPlayers, gameDuration, seasonInfo, gameNum, matchDictionary,
                    matchList, divisionId, championDetails);

                CollectMatchMvpData(view, matchList, registeredPlayers, gameInfo, mvpDetails, gameNum, updateMvpDetails, insertMvpDetails);

                insertDetailsList.AddRange(matchList.Select(x => x.MatchDetail));
                insertStatsList.AddRange(matchList.Select(x => x.PlayerStats));
                insertAchievementsList.AddRange(matchList.SelectMany(x => x.Achievements));
            }

            if (!await DeleteOldRecords(view))
            {
                return false;
            }

            var insertAchievementsResult = await _achievementRepository.InsertAsync(insertAchievementsList);
            var insertMvpResult = await _matchMvpRepository.CreateAsync(insertMvpDetails);
            var updateMvpResult = await _matchMvpRepository.UpdateAsync(updateMvpDetails);
            var insertStatsResult = await _playerStatsRepository.InsertAsync(insertStatsList);
            var insertDetailsResult = await _matchDetailRepository.InsertAsync(insertDetailsList);
            var insertChampionStatsResult = await _championStatsRepository.CreateAsync(championDetails);

            return insertStatsResult && insertDetailsResult && insertMvpResult && updateMvpResult && insertChampionStatsResult && insertAchievementsResult;
        }

        public void CollectBans(MatchSubmissionView view, Match riotMatch, ChampionListStatic champions,
            SeasonInfoEntity seasonInfo, Guid divisionId, List<ChampionStatsEntity> championDetails)
        {
            foreach (var ban in riotMatch.Teams.SelectMany(x => x.Bans))
            {
                var riotChampion = champions.Keys[ban.ChampionId].ToLowerInvariant();
                try
                {
                    var ourChampion = GlobalVariables.ChampionEnumCache.Get<string, LookupEntity>(riotChampion);
                    var bannedChampionStat =
                        CreateChampionStat(seasonInfo, divisionId, ourChampion.Id, view.ScheduleId);
                    championDetails.Add(bannedChampionStat);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error getting banned champion: {riotChampion}");
                }
            }
        }

        public async Task<bool> DeleteOldRecords(MatchSubmissionView view)
        {
            var deleteMatchDetailsResult = await _matchDetailRepository.DeleteAsync(view.ScheduleId);
            var deleteByScheduleResult = await _championStatsRepository.DeleteByScheduleAsync(view.ScheduleId);
            if (!deleteMatchDetailsResult || deleteByScheduleResult)
            {
                _logger.LogError("Unable to delete old MatchDetails");
                return false;
            }

            return true;
        }

        public void CollectMatchMvpData(MatchSubmissionView view, List<MatchDetailContract> matchList, Dictionary<string, SummonerInfoEntity> registeredPlayers,
            GameInfo gameInfo, Dictionary<int, MatchMvpEntity> mvpDetails, int gameNum, List<MatchMvpEntity> updateMvpDetails, 
            List<MatchMvpEntity> insertMvpDetails)
        {
            var validMvpPlayers = new List<Guid>();
            validMvpPlayers.AddRange(matchList.Select(x => x.MatchDetail.PlayerId));

            registeredPlayers.TryGetValue(gameInfo.BlueMvp.ToLowerInvariant(), out var blueMvp);
            registeredPlayers.TryGetValue(gameInfo.RedMvp.ToLowerInvariant(), out var redMvp);

            if (mvpDetails.TryGetValue(gameNum, out var mvpEntity))
            {
                if (!string.IsNullOrEmpty(gameInfo.BlueMvp) && blueMvp != null && blueMvp.Id != mvpEntity.BlueMvp &&
                    validMvpPlayers.Contains(blueMvp.Id))
                {
                    mvpEntity.BlueMvp = blueMvp.Id;
                }

                if (!string.IsNullOrEmpty(gameInfo.RedMvp) && redMvp != null && redMvp.Id != mvpEntity.RedMvp &&
                    validMvpPlayers.Contains(redMvp.Id))
                {
                    mvpEntity.RedMvp = redMvp.Id;
                }

                updateMvpDetails.Add(mvpEntity);
            }
            else
            {
                mvpEntity = new MatchMvpEntity
                {
                    Id = Guid.NewGuid(),
                    BlueMvp = blueMvp != null && validMvpPlayers.Contains(blueMvp.Id) ? blueMvp.Id : new Guid?(),
                    RedMvp = redMvp != null && validMvpPlayers.Contains(redMvp.Id) ? redMvp.Id : new Guid?(),
                    Game = gameNum,
                    TeamScheduleId = view.ScheduleId
                };
                insertMvpDetails.Add(mvpEntity);
            }
        }

        public async Task CollectPlayerMatchDetailsAsync(MatchSubmissionView view, Match riotMatch, ChampionListStatic champions, GameInfo gameInfo,
            Dictionary<string, SummonerInfoEntity> registeredPlayers, TimeSpan gameDuration, SeasonInfoEntity seasonInfo, int gameNum, 
            Dictionary<MatchDetailKey, MatchDetailEntity> matchDictionary, List<MatchDetailContract> matchList, Guid divisionId,
            List<ChampionStatsEntity> championDetails)
        {
            var blueTotalKills = riotMatch.Participants.Where(x => x.TeamId == 100).Sum(y => y.Stats.Kills);
            var redTotalKills = riotMatch.Participants.Where(x => x.TeamId == 200).Sum(y => y.Stats.Kills);

            foreach (var participant in riotMatch.Participants)
            {
                //Get champion by Riot Api
                var riotChampion = champions.Keys[participant.ChampionId].ToLowerInvariant();
                //Get who played said champion and if they were blue side or not
                var gameInfoPlayer = gameInfo.PlayerName(riotChampion);

                //If the player listed doesn't match a champion, then we ignore it for purposes of stat tracking
                if (gameInfoPlayer == null)
                {
                    continue;
                }

                //Check to make sure the player is officially registered, if not, this will send a red flag
                registeredPlayers.TryGetValue(gameInfoPlayer.PlayerName.ToLowerInvariant(), out var registeredPlayer);
                if (registeredPlayer == null)
                {
                    var message = $"This player is not legal for a match: {gameInfoPlayer.PlayerName}";
                    _logger.LogCritical(message);
                    const string to = "casualesportsamateurleague@gmail.com";
                    await _emailService.SendEmailAsync(to, message,
                        $"Illegal player in match: {view.HomeTeamName} vs {view.AwayTeamName}");
                    continue;
                }

                var matchStat = CreatePlayerMatchStat(registeredPlayer, participant, gameDuration, seasonInfo);
                switch (participant.TeamId)
                {
                    case 100:
                        matchStat.TotalTeamKills = (int)blueTotalKills;
                        break;
                    case 200:
                        matchStat.TotalTeamKills = (int)redTotalKills;
                        break;
                }

                //will always create a new match detail
                var matchDetail = new MatchDetailEntity
                {
                    Id = Guid.NewGuid(),
                    Game = gameNum,
                    PlayerId = registeredPlayer.Id,
                    PlayerStatsId = matchStat.Id,
                    SeasonInfoId = seasonInfo.Id,
                    TeamScheduleId = view.ScheduleId
                };

                //per player
                var win = participant.Stats.Winner;
                var loss = !win;
                var ourChampion = GlobalVariables.ChampionEnumCache.Get<string, LookupEntity>(riotChampion);

                var pickedChampionStat = CreateChampionStat(matchStat, seasonInfo, divisionId, win, loss, ourChampion.Id, matchDetail.Id, view.ScheduleId);
                championDetails.Add(pickedChampionStat);

                //Add special achievements here
                var achievements = await AddSpecialAchievements(participant, ourChampion, registeredPlayer, seasonInfo.Id, riotMatch, view, gameNum);
                matchList.Add(new MatchDetailContract(gameInfoPlayer.IsBlue, matchDetail, matchStat, achievements));
            }
        }

        public async Task<List<AchievementEntity>> AddSpecialAchievements(Participant player, LookupEntity ourChampion, SummonerInfoEntity summonerInfo, Guid seasonInfoId, Match riotMatch, MatchSubmissionView view, int currentGame)
        {
            var achievements = new List<AchievementEntity>();
            var teamName = "N/a";
            var currentTeamId = (await _teamPlayerRepository.GetBySummonerAndSeasonIdAsync(summonerInfo.Id, seasonInfoId))?.TeamRosterId;
            if (currentTeamId != null)
            {
                var playerTeam = await _teamRosterRepository.GetByTeamIdAsync(currentTeamId.Value);
                if (playerTeam.TeamName == view.HomeTeamName || playerTeam.TeamName == view.AwayTeamName)
                {
                    teamName = playerTeam.TeamName;
                }
            }

            if (player.Stats.LargestMultiKill >= 5)
            {
                var achivement = new AchievementEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = summonerInfo.UserId,
                    AchievedDate = DateTime.Today,
                    AchievedTeam = teamName,
                    Achievement = $"Penta-kill on {ourChampion.Value} in game {currentGame}"
                };
                achievements.Add(achivement);
            }

            var blueTeamPlayers = riotMatch.Participants.Where(x => x.TeamId == 100);
            var redTeamPlayers = riotMatch.Participants.Where(x => x.TeamId == 200);


            var blueTotalKills = blueTeamPlayers.Sum(y => y.Stats.Kills);
            var redTotalKills = redTeamPlayers.Sum(y => y.Stats.Kills);
            var isBlue = player.TeamId == 100;
            if (isBlue && redTotalKills == 0 || !isBlue && blueTotalKills == 0)
            {
                var blueTeam = riotMatch.Teams.First(x => x.TeamId == 100);
                var redTeam = riotMatch.Teams.First(x => x.TeamId == 200);
                if (blueTeam.DragonKills == 0 && blueTeam.BaronKills == 0 && blueTeam.TowerKills == 0 && !isBlue
                    || redTeam.DragonKills == 0 && redTeam.BaronKills == 0 && redTeam.TowerKills == 0 && isBlue)
                {
                    var achivement = new AchievementEntity
                    {
                        Id = Guid.NewGuid(),
                        UserId = summonerInfo.UserId,
                        AchievedDate = DateTime.Today,
                        AchievedTeam = teamName,
                        Achievement = $"Perfect Game on {ourChampion.Value} in game {currentGame}"
                    };
                    achievements.Add(achivement);
                }
            }

            try
            {
                var oldAchievements = (await _achievementRepository.GetAchievementsForUserAsync(summonerInfo.UserId)).ToList();
                var tempList = new List<AchievementEntity>(achievements);
                foreach (var newAchievement in tempList)
                {
                    var match = oldAchievements.FirstOrDefault(x => x.Equals(newAchievement));
                    if (match != null)
                    {
                        achievements.Remove(newAchievement);
                    }
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return achievements;
        }

        private static PlayerStatsEntity CreatePlayerMatchStat(SummonerInfoEntity registeredPlayer, Participant participant,
            TimeSpan gameDuration, SeasonInfoEntity seasonInfo)
        {
            var matchStat = new PlayerStatsEntity
            {
                Id = Guid.NewGuid(),
                SummonerId = registeredPlayer.Id,
                Kills = (int)participant.Stats.Kills,
                Deaths = (int)participant.Stats.Deaths,
                Assists = (int)participant.Stats.Assists,
                CS = (int)(participant.Stats.TotalMinionsKilled + participant.Stats.NeutralMinionsKilled),
                Gold = (int)participant.Stats.GoldEarned,
                VisionScore = (int)participant.Stats.VisionScore,
                GameTime = gameDuration,
                Games = 1,
                SeasonInfoId = seasonInfo.Id
            };
            return matchStat;
        }

        //per player
        private ChampionStatsEntity CreateChampionStat(PlayerStatsEntity playerStats, SeasonInfoEntity seasonInfo, Guid divisionId, bool win, bool loss, Guid championId, Guid matchDetailId, Guid teamScheduleId)
        {
            var championStat = new ChampionStatsEntity
            {
                Id = Guid.NewGuid(),
                PlayerId = playerStats.SummonerId,
                SeasonInfoId = seasonInfo.Id,
                DivisionId = divisionId,
                Kills = playerStats.Kills,
                Deaths = playerStats.Deaths,
                Assists = playerStats.Assists,
                Picked = true,
                Banned = false,
                Win = win,
                Loss = loss,
                ChampionId = championId,
                MatchDetailId = matchDetailId,
                TeamScheduleId = teamScheduleId
            };
            return championStat;
        }

        //per game
        private ChampionStatsEntity CreateChampionStat(SeasonInfoEntity seasonInfo, Guid divisionId, Guid championId, Guid teamScheduleId)
        {
            var championStat = new ChampionStatsEntity
            {
                Id = Guid.NewGuid(),
                PlayerId = null,
                SeasonInfoId = seasonInfo.Id,
                DivisionId = divisionId,
                Kills = 0,
                Assists = 0,
                Deaths = 0,
                Picked = false,
                Banned = true,
                Win = false,
                Loss = false,
                ChampionId = championId,
                TeamScheduleId = teamScheduleId
            };
            return championStat;
        }

        private string CreateCsvDataFile(MatchSubmissionView view)
        {
            var matchCsvsDir = Path.Combine(_wwwRootDirectory, "MatchCsvs");
            if (!Directory.Exists(matchCsvsDir))
            {
                Directory.CreateDirectory(matchCsvsDir);
            }

            var csvFile = Path.Combine(_wwwRootDirectory, $"MatchCsvs\\{view.FileName}-{Guid.NewGuid()}.csv");

            using (var writer = new StreamWriter(csvFile, false, Encoding.UTF8))
            {
                using (var csvWriter = new CsvWriter(writer))
                {
                    //csvWriter.WriteField("");

                    var gameNum = 0;
                    foreach (var gameInfo in view.GameInfos)
                    {
                        gameNum++;
                        WriteHeader(csvWriter, gameNum);
                        csvWriter.NextRecord();
                        for (var i = 0; i < 5; i++) //players
                        {
                            if (i == 0)
                            {
                                csvWriter.WriteField("");
                                csvWriter.WriteField(gameInfo.TeamWithSideSelection);
                                if (gameInfo.HomeTeamForfeit)
                                {
                                    csvWriter.WriteField("AwayTeam by HomeTeam forfeit");
                                    break;
                                }
                                if (gameInfo.AwayTeamForfeit)
                                {
                                    csvWriter.WriteField("HomeTeam by AwayTeam forfeit");
                                    break;
                                }
                                csvWriter.WriteField(gameInfo.BlueSideWinner ? "Blue" : "Red");
                                csvWriter.WriteField(gameInfo.ProdraftSpectateLink);
                                csvWriter.WriteField(gameInfo.MatchHistoryLink);

                                csvWriter.WriteField(gameInfo.BlueTeam.PlayerTop);
                                csvWriter.WriteField(gameInfo.BlueTeam.ChampionTop);
                                csvWriter.WriteField(gameInfo.RedTeam.PlayerTop);
                                csvWriter.WriteField(gameInfo.RedTeam.ChampionTop);
                            }
                            else if (i == 1)
                            {
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");

                                csvWriter.WriteField(gameInfo.BlueTeam.PlayerJungle);
                                csvWriter.WriteField(gameInfo.BlueTeam.ChampionJungle);
                                csvWriter.WriteField(gameInfo.RedTeam.PlayerJungle);
                                csvWriter.WriteField(gameInfo.RedTeam.ChampionJungle);
                            }

                            else if (i == 2)
                            {
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");

                                csvWriter.WriteField(gameInfo.BlueTeam.PlayerMid);
                                csvWriter.WriteField(gameInfo.BlueTeam.ChampionMid);
                                csvWriter.WriteField(gameInfo.RedTeam.PlayerMid);
                                csvWriter.WriteField(gameInfo.RedTeam.ChampionMid);
                            }

                            else if (i == 3)
                            {
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");

                                csvWriter.WriteField(gameInfo.BlueTeam.PlayerAdc);
                                csvWriter.WriteField(gameInfo.BlueTeam.ChampionAdc);
                                csvWriter.WriteField(gameInfo.RedTeam.PlayerAdc);
                                csvWriter.WriteField(gameInfo.RedTeam.ChampionAdc);
                            }

                            else if (i == 4)
                            {
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");
                                csvWriter.WriteField("");

                                csvWriter.WriteField(gameInfo.BlueTeam.PlayerSup);
                                csvWriter.WriteField(gameInfo.BlueTeam.ChampionSup);
                                csvWriter.WriteField(gameInfo.RedTeam.PlayerSup);
                                csvWriter.WriteField(gameInfo.RedTeam.ChampionSup);
                            }
                            csvWriter.NextRecord();
                        }
                        csvWriter.NextRecord();
                    }
                }
                writer.Close();
            }

            return csvFile;
        }

        private static void WriteHeader(IWriterRow csvWriter, int gameNum)
        {

            csvWriter.WriteField($"Game {gameNum}");
            var selected = gameNum % 2 == 1 ? "Home" : "Away";
            csvWriter.WriteField($"{selected} Side Selection");
            csvWriter.WriteField("Winner");
            csvWriter.WriteField("ProDraft Spectate Link");
            csvWriter.WriteField("Match History Link");
            csvWriter.WriteField("Blue Player");
            csvWriter.WriteField("Blue Champion");
            csvWriter.WriteField("Red Player");
            csvWriter.WriteField("Red Champion");

        }
    }
}
