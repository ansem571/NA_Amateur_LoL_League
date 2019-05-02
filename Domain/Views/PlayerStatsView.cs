using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Views
{
    public class PlayerStatsView
    {
        public Guid SummonerId { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public double CSperMin { get; set; }
        public double DamagePerMin { get; set; }
        public double Kp { get; set; }
        public int VisionScore { get; set; }
    }
}
