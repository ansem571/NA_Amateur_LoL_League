using System;
using System.Collections.Generic;
using System.Linq;
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
                AchievedTeam = form.TeamName
            };

            return await _achievementRepository.InsertAsync(achievement);
        }

        public async Task<PlayerProfileView> GetPlayerProfileAsync(Guid userId)
        {
            var seasonsTask = _seasonInfoRepository.GetAllSeasonsAsync();
            var summonerTask = _summonerInfoRepository.ReadOneByUserIdAsync(userId);
            var achievementsTask = _achievementRepository.GetAchievementsForUserAsync(userId);

            var summoner = await summonerTask;
            var achievements = await achievementsTask;

            var alternateAccountsTask = _alternateAccountRepository.ReadAllForSummonerAsync(summoner.Id);
            var teamPlayer = await _teamPlayerRepository.GetBySummonerIdAsync(summoner.Id);
            var teamname = "None";
            if (teamPlayer != null)
            {
                var team = await _teamRosterRepository.GetByTeamIdAsync(teamPlayer.TeamRosterId);
                teamname = team.TeamName;
            }

            var seasons = (await seasonsTask).OrderBy(x => x.SeasonStartDate).ToList();
            var playerStats = await _playerStatsRepository.GetStatsForSummonerAsync(summoner.Id, seasons.Last().Id);
            var mappedStats = new PlayerStatsView();
            if (playerStats != null)
            {
                mappedStats = _playerStatsMapper.Map(playerStats);
            }
            var alternateAccounts = await alternateAccountsTask;
            var altAccountsMapped = _alternateAccountMapper.Map(alternateAccounts);
            var achievementViews = new List<AchievementView>();
            foreach (var achievement in achievements)
            {
                var achievementView = new AchievementView(achievement);
                var seasonAchieved = seasons.FirstOrDefault(x =>
                    x.SeasonStartDate < achievement.AchievedDate &&
                    (x.SeasonEndDate ?? DateTime.MaxValue) > achievement.AchievedDate);
                achievementView.Season = seasonAchieved != null ? seasonAchieved.SeasonName : "PreSeason";
                achievementViews.Add(achievementView);
            }

            var view = new PlayerProfileView
            {
                PlayerName = summoner.SummonerName,
                Rank = _tierDivisionMapper.Map(summoner.Tier_DivisionId),
                TeamName = string.IsNullOrEmpty(teamname) ? "None" : teamname,
                Achievements = achievementViews,
                PlayerStats = mappedStats,
                AlternateAccountViews = altAccountsMapped
            };

            return view;
        }
    }
}
