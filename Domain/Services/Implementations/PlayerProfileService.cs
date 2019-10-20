using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using DAL.Entities.UserData;
using Domain.Forms;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.AspNetCore.Identity;

namespace Domain.Services.Implementations
{
    public class PlayerProfileService : IPlayerProfileService
    {
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly IAchievementRepository _achievementRepository;
        private readonly ITeamPlayerRepository _teamPlayerRepository;
        private readonly ITeamRosterRepository _teamRosterRepository;
        private readonly IAlternateAccountRepository _alternateAccountRepository;
        private readonly IPlayerStatsRepository _playerStatsRepository;
        private readonly ISeasonInfoRepository _seasonInfoRepository;
        private readonly IPlayerStatsMapper _playerStatsMapper;
        private readonly IAlternateAccountMapper _alternateAccountMapper;
        private readonly ITierDivisionMapper _tierDivisionMapper;

        //Skarner Alston Reztip pentakill 5/29/2019
        //Perfect Game ABCDE vs TDK semi finals 10/17/2019

        public PlayerProfileService(ISummonerInfoRepository summonerInfoRepository, IAchievementRepository achievementRepository, ITeamPlayerRepository teamPlayerRepository, 
            ITeamRosterRepository teamRosterRepository, IAlternateAccountRepository alternateAccountRepository, IPlayerStatsRepository playerStatsRepository, ISeasonInfoRepository seasonInfoRepository, 
            IPlayerStatsMapper playerStatsMapper, IAlternateAccountMapper alternateAccountMapper, ITierDivisionMapper tierDivisionMapper)
        {
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
            _achievementRepository = achievementRepository ?? throw new ArgumentNullException(nameof(achievementRepository));
            _teamPlayerRepository = teamPlayerRepository ?? throw new ArgumentNullException(nameof(teamPlayerRepository));
            _teamRosterRepository = teamRosterRepository ?? throw new ArgumentNullException(nameof(teamRosterRepository));
            _alternateAccountRepository = alternateAccountRepository ?? throw new ArgumentNullException(nameof(alternateAccountRepository));
            _playerStatsRepository = playerStatsRepository ?? throw new ArgumentNullException(nameof(playerStatsRepository));
            _seasonInfoRepository = seasonInfoRepository ?? throw new ArgumentNullException(nameof(seasonInfoRepository));
            _playerStatsMapper = playerStatsMapper ?? throw new ArgumentNullException(nameof(playerStatsMapper));
            _alternateAccountMapper = alternateAccountMapper ?? throw new ArgumentNullException(nameof(alternateAccountMapper));
            _tierDivisionMapper = tierDivisionMapper ?? throw new ArgumentNullException(nameof(tierDivisionMapper));
        }

        public async Task<bool> InsertAchievement(UserAchievementForm form)
        {
            var achievement = new AchievementEntity
            {
                Id = Guid.NewGuid(),
                UserId = form.UserId,
                Achievement = form.Achievement,
                AchievedDate = form.Date,
                AchievedTeam = form.TeamName,
                AchievedAgainst = form.AgainstTeam
            };

            return await _achievementRepository.InsertAsync(achievement);
        }

        public async Task<PlayerProfileView> GetPlayerProfileAsync(Guid userId)
        {
            var seasonInfoTask = _seasonInfoRepository.GetActiveSeasonInfoByDate(DateTime.Today);
            var summonerTask = _summonerInfoRepository.ReadOneByUserIdAsync(userId);
            var achievementsTask = _achievementRepository.GetAchievementsForUserAsync(userId);

            var summoner = await summonerTask;
            var achievements = await achievementsTask;

            var alternateAccountsTask = _alternateAccountRepository.ReadAllForSummonerAsync(summoner.Id);
            var teamPlayer = await _teamPlayerRepository.GetBySummonerIdAsync(summoner.Id);
            var team = await _teamRosterRepository.GetByTeamIdAsync(teamPlayer.TeamRosterId);

            var seasonInfo = await seasonInfoTask;
            var playerStats = await _playerStatsRepository.GetStatsForSummonerAsync(summoner.Id, seasonInfo.Id);
            var mappedStats = _playerStatsMapper.Map(playerStats);
            var alternateAccounts = await alternateAccountsTask;
            var altAccountsMapped = _alternateAccountMapper.Map(alternateAccounts);

            var view = new PlayerProfileView
            {
                PlayerName = summoner.SummonerName,
                Rank = _tierDivisionMapper.Map(summoner.Tier_DivisionId),
                TeamName = string.IsNullOrEmpty(team.TeamName) ? "None" : team.TeamName,
                Achievements = achievements,
                PlayerStats = mappedStats,
                AlternateAccountViews = altAccountsMapped
            };

            return view;
        }
    }
}
