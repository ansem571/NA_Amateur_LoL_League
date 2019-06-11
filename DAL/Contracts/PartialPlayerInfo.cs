using System;

namespace DAL.Contracts
{
    public class PartialPlayerInfo
    {
        public string Name { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int CS { get; set; }
        public int Gold { get; set; }
        public int VisionScore { get; set; }
        public int RealVisionScore => VisionScore / Games;
        public TimeSpan Duration { get; set; }
        public int TotalTeamKills { get; set; }
        public int Games { get; set; } = 1;

        public PartialPlayerInfo()
        {
        }

        public void AddOtherGame(PartialPlayerInfo otherStats)
        {
            Games++;
            Kills += otherStats.Kills;
            Deaths += otherStats.Deaths;
            Assists += otherStats.Assists;
            CS += otherStats.CS;
            Gold += otherStats.Gold;
            VisionScore += otherStats.VisionScore;
            Duration += otherStats.Duration;
            TotalTeamKills += otherStats.TotalTeamKills;
        }
    }
}
