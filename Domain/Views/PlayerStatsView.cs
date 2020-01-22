using System;

namespace Domain.Views
{
    public class PlayerStatsView
    {
        public Guid SummonerId { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public double Kda { get; set; }
        public double CSperMin { get; set; }
        public double DamagePerMin { get; set; }
        public double Kp { get; set; }
        public double VisionScore { get; set; }
        public Guid? SeasonInfoId { get; set; }
        public double MvpVotes { get; set; }


        public PlayerStatsView()
        {
        }

        public static PlayerStatsView operator +(PlayerStatsView current, PlayerStatsView other)
        {
            if (current.SummonerId != other.SummonerId)
            {
                throw new Exception("Invalid summoner stats");
            }
            return new PlayerStatsView
            {
                SummonerId = current.SummonerId,
                Kills = current.Kills + other.Kills,
                Deaths = current.Deaths + other.Deaths,
                Assists = current.Assists + other.Assists,
            };
        }
    }
}
