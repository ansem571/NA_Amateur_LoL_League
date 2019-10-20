using System;
using System.Collections.Generic;
using System.Text;
using DAL.Entities.LeagueInfo;
using Domain.Enums;

namespace Domain.Views
{
    public class PlayerProfileView
    {
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public TierDivisionEnum Rank { get; set; }
        public IEnumerable<AchievementEntity> Achievements { get; set; }
        public PlayerStatsView PlayerStats { get; set; }
        public IEnumerable<AlternateAccountView> AlternateAccountViews { get; set; }
    }
}
