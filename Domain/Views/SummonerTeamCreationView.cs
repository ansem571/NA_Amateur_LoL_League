using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Views
{
    public class SummonerTeamCreationView
    {
        public List<SummonerInfo> SummonerInfos { get; set; }
    }

    public class SummonerInfo
    {
        public Guid SummonerId { get; set; }
        public string SummonerName { get; set; }
    }
}
