using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Enums;

namespace Domain.Views
{
    public class RosterView
    {
        public Guid RosterId { get; set; }
        public string Captain { get; set; }
        public string TeamName { get; set; }
        public int TeamTierScore { get; set; }
        public DivisionView Division { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public int Points { get; set; }
        public IEnumerable<DetailedSummonerInfoView> Players { get; set; } = new List<DetailedSummonerInfoView>();
        public string FileSource { get; set; }
        public IEnumerable<ScheduleView> Schedule { get; set; }

        public void Cleanup()
        {
            var tempDictionary = new Dictionary<SummonerRoleEnum, DetailedSummonerInfoView>
            {
                {SummonerRoleEnum.Top, new DetailedSummonerInfoView() },
                {SummonerRoleEnum.Jungle, new DetailedSummonerInfoView() },
                {SummonerRoleEnum.Mid, new DetailedSummonerInfoView() },
                {SummonerRoleEnum.Adc, new DetailedSummonerInfoView() },
                {SummonerRoleEnum.Sup, new DetailedSummonerInfoView() }
            };

            var unUsedList = new List<DetailedSummonerInfoView>(Players);

            var firstTop = unUsedList.FirstOrDefault(x => x.TeamRole == SummonerRoleEnum.Top && !x.IsSub);
            if (firstTop != null)
            {
                tempDictionary[SummonerRoleEnum.Top] = firstTop;
                unUsedList.Remove(firstTop);
            }

            var firstJungle = unUsedList.FirstOrDefault(x => x.TeamRole == SummonerRoleEnum.Jungle && !x.IsSub);
            if (firstJungle != null)
            {
                tempDictionary[SummonerRoleEnum.Jungle] = firstJungle;
                unUsedList.Remove(firstJungle);
            }

            var firstMid = unUsedList.FirstOrDefault(x => x.TeamRole == SummonerRoleEnum.Mid && !x.IsSub);
            if (firstMid != null)
            {
                tempDictionary[SummonerRoleEnum.Mid] = firstMid;
                unUsedList.Remove(firstMid);
            }

            var firstAdc = unUsedList.FirstOrDefault(x => x.TeamRole == SummonerRoleEnum.Adc && !x.IsSub);
            if (firstAdc != null)
            {
                tempDictionary[SummonerRoleEnum.Adc] = firstAdc;
                unUsedList.Remove(firstAdc);
            }

            var firstSup = unUsedList.FirstOrDefault(x => x.TeamRole == SummonerRoleEnum.Sup && !x.IsSub);
            if (firstSup != null)
            {
                tempDictionary[SummonerRoleEnum.Sup] = firstSup;
                unUsedList.Remove(firstSup);
            }

            var tempList = tempDictionary.Values.ToList();
            tempList.AddRange(unUsedList);
            Players = tempList;
        }
    }
}
