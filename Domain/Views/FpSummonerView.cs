using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Views
{
    public class FpSummonerView
    {
        public List<FpSummonerInfo> SummonerInfos { get; set; }= new List<FpSummonerInfo>();
    }

    public class FpSummonerInfo
    {
        public Guid UserId { get; set; }
        public string SummonerName { get; set; }
        public bool IsTeamCaptain { get; set; }
        public SummonerRoleEnum Role { get; set; }
        public SummonerRoleEnum OffRole { get; set; }
        public TierDivisionEnum TierDivision { get; set; }
        public TierDivisionEnum? PreviousSeasonTierDivision { get; set; }
        public string OpGgUrl { get; set; }
        public string TeamName { get; set; }
        public Guid RosterId { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsEsubOnly { get; set; }
        public bool IsAcademyPlayer { get; set; }
    }
}
