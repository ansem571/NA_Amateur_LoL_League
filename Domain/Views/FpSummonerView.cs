using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Views
{
    public class FpSummonerView
    {
        public List<FpSummonerInfo> SummonerInfos { get; set; }= new List<FpSummonerInfo>();
    }

    public class FpSummonerInfo
    {
        public string SummonerName { get; set; }
        public SummonerRoleEnum Role { get; set; }
        public SummonerRoleEnum OffRole { get; set; }
        public TierDivisionEnum TierDivision { get; set; }
        public string OpGgUrl { get; set; }
        public string TeamName { get; set; }
    }
}
