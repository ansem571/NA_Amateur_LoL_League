using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Views
{
    public class PlayerProfileView
    {
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public TierDivisionEnum Rank { get; set; }
        public IEnumerable<AchievementView> Achievements { get; set; }
        public Dictionary<int, PlayerStatsView> PlayerStats { get; set; }
        //public PlayerStatsView PlayerStats { get; set; }
        public IEnumerable<AlternateAccountView> AlternateAccountViews { get; set; }
    }
}
