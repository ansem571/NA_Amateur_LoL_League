using System;

namespace Domain.Views
{
    public class DetailedSummonerInfoView : SummonerInfoView
    {
        public Guid Id { get; set; }
        public PlayerStatsView PlayerStats { get; set; }
        public bool IsSub { get; set; }
    }
}
