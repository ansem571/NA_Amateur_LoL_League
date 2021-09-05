using DAL.Entities.LeagueInfo;
using DAL.Entities.UserData;
using Domain.Enums;
using Domain.Helpers;
using Domain.Repositories.Implementations;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Implementations
{

    public class GameInfoService : IGameInfoService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ITeamRosterRepository _teamRosterRepository;
        //private readonly IMatchDetailRepository _matchDetailRepository;
        //private readonly IMatchMvpRepository _matchMvpRepository;
        //private readonly IChampionStatsRepository _championStatsRepository;
        //private readonly ILookupRepository _lookupRepository;
        //private readonly ISummonerInfoRepository _summonerInfoRepository;

        public GameInfoService(IScheduleRepository scheduleRepository, ITeamRosterRepository teamRosterRepository)
        {
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _teamRosterRepository = teamRosterRepository ?? throw new ArgumentNullException(nameof(teamRosterRepository));
        }

        public async Task<MatchInfo> GetGameInfoForMatch(Guid scheduleId)
        {
            var matchInfo = new MatchInfo();

            var match = await _scheduleRepository.GetScheduleAsync(scheduleId);
            var homeTeam = await _teamRosterRepository.GetByTeamIdAsync(match.HomeRosterTeamId);
            var awayTeam = await _teamRosterRepository.GetByTeamIdAsync(match.AwayRosterTeamId);

            matchInfo.HomeTeam = homeTeam.TeamName;
            matchInfo.AwayTeam = awayTeam.TeamName;
            matchInfo.ScheduleId = scheduleId;

            return matchInfo;

            //Re enable for testing

            //var rolesTask = _lookupRepository.GetLookupEntitiesByCategory("Role");
            //var championsTask = _lookupRepository.GetLookupEntitiesByCategory("Champion");
            //var matchDetailsTask = _matchDetailRepository.ReadForScheduleId(scheduleId);
            //var mvpStatsTask = _matchMvpRepository.ReadAllForTeamScheduleId(scheduleId);
            //var registeredPlayersTask = _summonerInfoRepository.GetAllValidSummonersAsync();

            //var matchDetails = await matchDetailsTask;
            //var championsPlayedTask = _championStatsRepository.GetChampionStatsForMatchDetailAsync(matchDetails.Values.Select(x => x.Id));

            //var groupedMatchDetails = matchDetails.GroupBy(x => x.Key.Game).OrderBy(x => x.Key)
            //                                        .ToDictionary(x => x.Key, x => x.ToList());

            //var mvpStats = (await mvpStatsTask).OrderBy(x => x.Game).ToList();
            //var championsPlayed = (await championsPlayedTask).ToDictionary(x => x.MatchDetailId, x => x);
            //var roles = (await rolesTask).ToDictionary(x => x.Id, x => x);
            //var champions = (await championsTask).ToDictionary(x => x.Id, x => x);
            //var registeredPlayers = (await registeredPlayersTask).ToDictionary(x => x.Id, x => x);

            //SetupGameInfos(homeTeam, awayTeam, groupedMatchDetails, mvpStats, championsPlayed, roles, champions, registeredPlayers);

            //return matchInfo;
        }

        //private void SetupGameInfos(TeamRosterEntity homeTeam, TeamRosterEntity awayTeam, 
        //    Dictionary<int, List<KeyValuePair<MatchDetailKey, MatchDetailEntity>>> groupedMatchDetails,
        //    List<MatchMvpEntity> mvpStats, Dictionary<Guid?, ChampionStatsEntity> championsPlayed, 
        //    Dictionary<Guid, LookupEntity> roles, Dictionary<Guid, LookupEntity> champions, 
        //    Dictionary<Guid, SummonerInfoEntity> registeredPlayers)
        //{
        //    var gameNum = 0;
        //    foreach (var gameMatchDetails in groupedMatchDetails)
        //    {
        //        gameNum++;
        //        var gameInfo = new GameInfo();
        //        gameInfo.GameNum = gameMatchDetails.Key;
        //        gameInfo.TeamWithSideSelection = gameMatchDetails.Key % 2 == 1 ? homeTeam.TeamName : awayTeam.TeamName;

        //        if (mvpStats[gameNum - 1].BlueMvp != null)
        //        {
        //            registeredPlayers.TryGetValue(mvpStats[gameNum - 1].BlueMvp.Value, out var registeredPlayer);
        //            gameInfo.BlueMvp = registeredPlayer.SummonerName;
        //        }
        //        if (mvpStats[gameNum - 1].RedMvp != null)
        //        {
        //            registeredPlayers.TryGetValue(mvpStats[gameNum - 1].RedMvp.Value, out var registeredPlayer);
        //            gameInfo.RedMvp = registeredPlayer.SummonerName;
        //        }

        //        bool blueSideWinnerSet = false;
        //        foreach (var matchDetailKvp in gameMatchDetails.Value)
        //        {
        //            var matchDetail = matchDetailKvp.Value;
        //            var isBlueSide = matchDetail.Blue == true;
        //            if (matchDetail.Winner && !blueSideWinnerSet)
        //            {
        //                gameInfo.BlueSideWinner = isBlueSide;
        //                blueSideWinnerSet = true;
        //            }

        //            roles.TryGetValue(matchDetail.RoleId.GetValueOrDefault(), out var roleLookup);
        //            registeredPlayers.TryGetValue(matchDetail.PlayerId, out var player);
        //            SummonerRoleEnum role = SummonerRoleEnum.None;
        //            if (roleLookup != null)
        //            {
        //                role = Enum.Parse<SummonerRoleEnum>(roleLookup.Enum);
        //                switch (role)
        //                {
        //                    case SummonerRoleEnum.Top:
        //                        {
        //                            if (isBlueSide)
        //                            {
        //                                gameInfo.BlueTeam.PlayerTop = player.SummonerName;
        //                            }
        //                            else
        //                            {
        //                                gameInfo.RedTeam.PlayerTop = player.SummonerName;
        //                            }
        //                        }
        //                        break;
        //                    case SummonerRoleEnum.Jungle:
        //                        {
        //                            if (isBlueSide)
        //                            {
        //                                gameInfo.BlueTeam.PlayerJungle = player.SummonerName;
        //                            }
        //                            else
        //                            {
        //                                gameInfo.RedTeam.PlayerJungle = player.SummonerName;
        //                            }
        //                        }
        //                        break;
        //                    case SummonerRoleEnum.Mid:
        //                        {
        //                            if (isBlueSide)
        //                            {
        //                                gameInfo.BlueTeam.PlayerMid = player.SummonerName;
        //                            }
        //                            else
        //                            {
        //                                gameInfo.RedTeam.PlayerMid = player.SummonerName;
        //                            }
        //                        }
        //                        break;
        //                    case SummonerRoleEnum.Adc:
        //                        {
        //                            if (isBlueSide)
        //                            {
        //                                gameInfo.BlueTeam.PlayerAdc = player.SummonerName;
        //                            }
        //                            else
        //                            {
        //                                gameInfo.RedTeam.PlayerAdc = player.SummonerName;
        //                            }
        //                        }
        //                        break;
        //                    case SummonerRoleEnum.Sup:
        //                        {
        //                            if (isBlueSide)
        //                            {
        //                                gameInfo.BlueTeam.PlayerSup = player.SummonerName;
        //                            }
        //                            else
        //                            {
        //                                gameInfo.RedTeam.PlayerSup = player.SummonerName;
        //                            }
        //                        }
        //                        break;
        //                    default:
        //                        {
        //                            throw new ArgumentOutOfRangeException("No Role for that on a team in match submission");
        //                        }
        //                }
        //            }
        //            championsPlayed.TryGetValue(matchDetail.Id, out var chamionEntity);
        //            champions.TryGetValue(chamionEntity.Id, out var championLookup);
        //            if (championLookup != null)
        //            {
        //                var champion = Enum.Parse<ChampionsEnum>(championLookup.Enum);
        //                switch (role)
        //                {
        //                    case SummonerRoleEnum.Top:
        //                        {
        //                            if (isBlueSide)
        //                            {
        //                                gameInfo.BlueTeam.ChampionTop = champion;
        //                            }
        //                            else
        //                            {
        //                                gameInfo.RedTeam.ChampionTop = champion;
        //                            }
        //                        }
        //                        break;
        //                    case SummonerRoleEnum.Jungle:
        //                        {
        //                            if (isBlueSide)
        //                            {
        //                                gameInfo.BlueTeam.ChampionJungle = champion;
        //                            }
        //                            else
        //                            {
        //                                gameInfo.RedTeam.ChampionJungle = champion;
        //                            }
        //                        }
        //                        break;
        //                    case SummonerRoleEnum.Mid:
        //                        {
        //                            if (isBlueSide)
        //                            {
        //                                gameInfo.BlueTeam.ChampionMid = champion;
        //                            }
        //                            else
        //                            {
        //                                gameInfo.RedTeam.ChampionMid = champion;
        //                            }
        //                        }
        //                        break;
        //                    case SummonerRoleEnum.Adc:
        //                        {
        //                            if (isBlueSide)
        //                            {
        //                                gameInfo.BlueTeam.ChampionAdc = champion;
        //                            }
        //                            else
        //                            {
        //                                gameInfo.RedTeam.ChampionAdc = champion;
        //                            }
        //                        }
        //                        break;
        //                    case SummonerRoleEnum.Sup:
        //                        {
        //                            if (isBlueSide)
        //                            {
        //                                gameInfo.BlueTeam.ChampionSup = champion;
        //                            }
        //                            else
        //                            {
        //                                gameInfo.RedTeam.ChampionSup = champion;
        //                            }
        //                        }
        //                        break;
        //                    default:
        //                        {
        //                            throw new ArgumentOutOfRangeException("No Role for that on a team in match submission");
        //                        }
        //                }
        //            }


        //        }

        //    }
        //}
    }
}
