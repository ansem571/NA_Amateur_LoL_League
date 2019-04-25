using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Enums;

namespace Domain.Views
{
    public class RosterView
    {
        public string Captain { get; set; }
        public string TeamName { get; set; }
        public int TeamTierScore { get; set; }
        public string DivisionName { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public IEnumerable<SummonerInfoView> Players { get; set; } = new List<SummonerInfoView>();

        public void Cleanup()
        {
            var tempDictionary = new Dictionary<SummonerRoleEnum, SummonerInfoView>
            {
                {SummonerRoleEnum.Top, new SummonerInfoView() },
                {SummonerRoleEnum.Jungle, new SummonerInfoView() },
                {SummonerRoleEnum.Mid, new SummonerInfoView() },
                {SummonerRoleEnum.Adc, new SummonerInfoView() },
                {SummonerRoleEnum.Sup, new SummonerInfoView() }
            };

            var unUsedList = new List<SummonerInfoView>(Players);

            var firstTop = unUsedList.FirstOrDefault(x => x.Role == SummonerRoleEnum.Top);
            if (firstTop != null)
            {
                tempDictionary[SummonerRoleEnum.Top] = firstTop;
                unUsedList.Remove(firstTop);
            }

            var firstJungle = unUsedList.FirstOrDefault(x => x.Role == SummonerRoleEnum.Jungle);
            if (firstJungle != null)
            {
                tempDictionary[SummonerRoleEnum.Jungle] = firstJungle;
                unUsedList.Remove(firstJungle);
            }

            var firstMid = unUsedList.FirstOrDefault(x => x.Role == SummonerRoleEnum.Mid);
            if (firstMid != null)
            {
                tempDictionary[SummonerRoleEnum.Mid] = firstMid;
                unUsedList.Remove(firstMid);
            }

            var firstAdc = unUsedList.FirstOrDefault(x => x.Role == SummonerRoleEnum.Adc);
            if (firstAdc != null)
            {
                tempDictionary[SummonerRoleEnum.Adc] = firstAdc;
                unUsedList.Remove(firstAdc);
            }

            var firstSup = unUsedList.FirstOrDefault(x => x.Role == SummonerRoleEnum.Sup);
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
