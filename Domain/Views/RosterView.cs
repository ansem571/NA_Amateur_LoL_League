using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Views
{
    public class RosterView
    {
        public string Captain { get; set; }
        public string TeamName { get; set; }
        public int TeamTierScore { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public IEnumerable<SummonerInfoView> Players { get; set; } = new List<SummonerInfoView>();
    }
}
