using System.Collections.Generic;
using DAL.Entities.LeagueInfo;

namespace Domain.Views
{
    public class MatchDetailContract
    {
        public bool IsBlue { get; set; }
        public MatchDetailEntity MatchDetail { get; set; }
        public PlayerStatsEntity PlayerStats { get; set; }
        public List<AchievementEntity> Achievements { get; set; }

        public MatchDetailContract() { }

        public MatchDetailContract(bool isBlue, MatchDetailEntity matchDetail, PlayerStatsEntity playerStats, List<AchievementEntity> achievements)
        {
            IsBlue = isBlue;
            MatchDetail = matchDetail;
            PlayerStats = playerStats;
            Achievements = achievements;
        }

    }
}
