using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using DAL.Contracts;
using DAL.Entities.LeagueInfo;
using DAL.Entities.Logging;
using Domain.Enums;
using Domain.Helpers;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Logging;
using RiotSharp.Endpoints.MatchEndpoint;
using RiotSharp.Endpoints.StaticDataEndpoint;

namespace Domain.Services.Implementations
{
    public class MatchDetailService : IMatchDetailService
    {
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;
        private readonly ISeasonInfoRepository _seasonInfoRepository;
        private readonly ITeamRosterRepository _teamRosterRepository;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly ITeamPlayerRepository _teamPlayerRepository;
        private readonly IPlayerStatsRepository _playerStatsRepository;
        private readonly IMatchDetailRepository _matchDetailRepository;

        private readonly string _wwwRootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        public MatchDetailService(ILogger logger, IEmailService emailService, IPlayerStatsRepository playerStatsRepository,
            ITeamPlayerRepository teamPlayerRepository, ISummonerInfoRepository summonerInfoRepository, ITeamRosterRepository teamRosterRepository,
            ISeasonInfoRepository seasonInfoRepository, IMatchDetailRepository matchDetailRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _playerStatsRepository = playerStatsRepository ?? throw new ArgumentNullException(nameof(playerStatsRepository));
            _teamPlayerRepository = teamPlayerRepository ?? throw new ArgumentNullException(nameof(teamPlayerRepository));
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
            _teamRosterRepository = teamRosterRepository ?? throw new ArgumentNullException(nameof(teamRosterRepository));
            _seasonInfoRepository = seasonInfoRepository ?? throw new ArgumentNullException(nameof(seasonInfoRepository));
            _matchDetailRepository = matchDetailRepository ?? throw new ArgumentNullException(nameof(matchDetailRepository));
        }

        public async Task<bool> SendFileData(MatchSubmissionView view)
        {
            var addMatchStats = await UpdateStatsAsync(view);
            if (!addMatchStats)
            {
                return false;
            }

            var csvDataFile = CreateCsvDataFile(view);

            //TODO: Will send file out to email as a personal record / to put on our Record of Games doc
            await _emailService.SendEmailAsync("casualesportsamateurleague@gmail.com", "Match result for subject", view.FileName, new List<Attachment>
            {
                new Attachment(csvDataFile)
            });

            return true;
        }

        private async Task<bool> UpdateStatsAsync(MatchSubmissionView view)
        {
            var matchesTask = _matchDetailRepository.ReadForScheduleId(view.ScheduleId);
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDateAsync(TimeZoneExtensions.GetCurrentTime().Date);
            var teamsTask = _teamRosterRepository.GetAllTeamsAsync(seasonInfo.Id);
            var registeredPlayersTask = _summonerInfoRepository.GetAllSummonersAsync();
            var teams = (await teamsTask).ToDictionary(x => x.TeamName, x => x);
            var registeredPlayers = (await registeredPlayersTask).ToDictionary(x => x.SummonerName.ToLowerInvariant(), x => x);
            var matchDictionary = (await matchesTask);
            var teamPlayers = (await _teamPlayerRepository.ReadAllAsync()).ToList();

            teams.TryGetValue(view.HomeTeamName, out var dbTeam1);
            teams.TryGetValue(view.AwayTeamName, out var dbTeam2);


            var gameNum = 0;
            //Will always insert new records and never update, we will delete any old records first
            var insertDetailsList = new List<MatchDetailEntity>();
            var insertStatsList = new List<PlayerStatsEntity>();
            //List of Ids to hard delete
            var deleteList = new List<Guid>();
            foreach (var gameInfo in view.GameInfos)
            {
                gameNum++;

                if (gameInfo.HomeTeamForfeit || gameInfo.AwayTeamForfeit)
                {
                    continue;
                }


                //There will always ever be 10 players per match
                //matchhistory.na.leagueoflegends.com/en/#match-details/NA1/{match id}/{dont care}?tab=overview
                var split = gameInfo.MatchHistoryLink.Split("/");
                if (!uint.TryParse(split[4], out var matchId))
                {
                    return false;
                }

                var version = (await GlobalVariables.ChampsApi.Versions.GetAllAsync()).First();
                var championsTask = GlobalVariables.ChampsApi.Champions.GetAllAsync(version);
                var riotMatchTask = GlobalVariables.Api.Match.GetMatchAsync(RiotSharp.Misc.Region.Na, matchId);

                var riotMatch = await riotMatchTask;
                var champions = await championsTask;

                var gameDuration = riotMatch.GameDuration;

                var redTotalKills = 0;
                var blueTotalKills = 0;

                var matchList = new List<(bool isBlue, MatchDetailEntity matchDetail, PlayerStatsEntity playerStats)>();
                foreach (var participant in riotMatch.Participants)
                {
                    //Get champion by Riot Api
                    var riotChampion = champions.Keys[participant.ChampionId];
                    //Get who played said champion and if they were blue side or not
                    var gameInfoPlayer = gameInfo.PlayerName(riotChampion);

                    //Adds to total kills for teams (this is used in team total kill calculation for % kill participation
                    AddToTotalKills(gameInfoPlayer, ref blueTotalKills, participant, ref redTotalKills);

                    //Check to make sure the player is officially registered, if not, this will send a red flag
                    registeredPlayers.TryGetValue(gameInfoPlayer.playerName.ToLowerInvariant(), out var registeredPlayer);
                    if (registeredPlayer == null)
                    {
                        _logger.LogCritical($"This player is not legal: {gameInfoPlayer.playerName}");
                        continue;
                    }

                    //TODO: what should we do here?
                    if (dbTeam1 != null && dbTeam2 != null)
                    {
                        var teamPlayer = teamPlayers.FirstOrDefault(x =>
                            x.SummonerId == registeredPlayer.Id && (x.TeamRosterId == dbTeam1.Id || x.TeamRosterId == dbTeam2.Id));

                        if (teamPlayer == null)
                        {
                            //Player was not on team
                        }

                    }
                    //
                    var matchStat = CreatePlayerMatchStat(registeredPlayer, participant, gameDuration, seasonInfo);

                    var matchDetailKey = new MatchDetailKey(view.ScheduleId, gameNum, registeredPlayer.Id);

                    //If the match detail exists, we delete the record so it will not be used in calculations later
                    if (matchDictionary.TryGetValue(matchDetailKey, out var oldMatchDetail))
                    {
                        deleteList.Add(oldMatchDetail.Id);
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

                    matchList.Add((gameInfoPlayer.isBlue, matchDetail, matchStat));
                }

                foreach (var tuple in matchList)
                {
                    if (tuple.isBlue)
                    {
                        tuple.playerStats.TotalTeamKills = blueTotalKills;
                    }
                    else
                    {
                        tuple.playerStats.TotalTeamKills = redTotalKills;
                    }
                }

                insertDetailsList.AddRange(matchList.Select(x => x.matchDetail));
                insertStatsList.AddRange(matchList.Select(x => x.playerStats));
            }

            var deleteResult = await _matchDetailRepository.DeleteAsync(deleteList);
            if (!deleteResult)
            {
                _logger.LogError("Unable to delete old MatchDetails");
                return false;
            }

            var insertDetailsResultTask = _matchDetailRepository.InsertAsync(insertDetailsList);
            var insertStatsResultTask = _playerStatsRepository.InsertAsync(insertStatsList);

            return await insertDetailsResultTask && await insertStatsResultTask;
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

        private void AddToTotalKills((bool isBlue, string playerName) gameInfoPlayer, ref int blueTotalKills,
            Participant participant, ref int redTotalKills)
        {
            if (gameInfoPlayer.isBlue)
            {
                blueTotalKills += (int)participant.Stats.Kills;
            }
            else
            {
                redTotalKills += (int)participant.Stats.Kills;
            }
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
