namespace Domain.Views
{
    public class DetailedSummonerInfoView : SummonerInfoView
    {
        public PlayerStatsView PlayerStats { get; set; }
        public bool IsSub { get; set; }
    }
}
