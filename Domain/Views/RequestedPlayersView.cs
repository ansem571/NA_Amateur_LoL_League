using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Enums;

namespace Domain.Views
{
    public class RequestedPlayersView
    {
        public List<PartialSummonerView> Summoners { get; set; }
        public List<SummonerRoleEnum> LookingForSummonerRoles => LookingForRoles();
        public string LookingForString => string.Join(", ", LookingForSummonerRoles);

        private List<SummonerRoleEnum> LookingForRoles()
        {
            var listOfRoles = Enum.GetValues(typeof(SummonerRoleEnum)).Cast<SummonerRoleEnum>().ToList();
            listOfRoles.Remove(SummonerRoleEnum.None);
            listOfRoles.Remove(SummonerRoleEnum.Fill);

            foreach (var partialSummonerView in Summoners)
            {
                if (listOfRoles.Contains(partialSummonerView.RoleForTeam))
                {
                    listOfRoles.Remove(partialSummonerView.RoleForTeam);
                }
            }

            return listOfRoles;
        }

        public void CleanupList()
        {
            var tempDictionary = new Dictionary<SummonerRoleEnum, PartialSummonerView>
            {
                {SummonerRoleEnum.Top, new PartialSummonerView() },
                {SummonerRoleEnum.Jungle, new PartialSummonerView() },
                {SummonerRoleEnum.Mid, new PartialSummonerView() },
                {SummonerRoleEnum.Adc, new PartialSummonerView() },
                {SummonerRoleEnum.Sup, new PartialSummonerView() }
            };

            var unUsedList = new List<PartialSummonerView>(Summoners);

            var firstTop = unUsedList.FirstOrDefault(x => x.RoleForTeam == SummonerRoleEnum.Top);
            if (firstTop != null)
            {
                tempDictionary[SummonerRoleEnum.Top] = firstTop;
                unUsedList.Remove(firstTop);
            }

            var firstJungle = unUsedList.FirstOrDefault(x => x.RoleForTeam == SummonerRoleEnum.Jungle);
            if (firstJungle != null)
            {
                tempDictionary[SummonerRoleEnum.Jungle] = firstJungle;
                unUsedList.Remove(firstJungle);
            }

            var firstMid = unUsedList.FirstOrDefault(x => x.RoleForTeam == SummonerRoleEnum.Mid);
            if (firstMid != null)
            {
                tempDictionary[SummonerRoleEnum.Mid] = firstMid;
                unUsedList.Remove(firstMid);
            }

            var firstAdc = unUsedList.FirstOrDefault(x => x.RoleForTeam == SummonerRoleEnum.Adc);
            if (firstAdc != null)
            {
                tempDictionary[SummonerRoleEnum.Adc] = firstAdc;
                unUsedList.Remove(firstAdc);
            }

            var firstSup = unUsedList.FirstOrDefault(x => x.RoleForTeam == SummonerRoleEnum.Sup);
            if (firstSup != null)
            {
                tempDictionary[SummonerRoleEnum.Sup] = firstSup;
                unUsedList.Remove(firstSup);
            }

            var tempList = tempDictionary.Values.ToList();
            tempList.AddRange(unUsedList);
            Summoners = tempList;
        }
    }

    public class PartialSummonerView
    {
        public string SummonerName { get; set; }
        public SummonerRoleEnum RoleForTeam { get; set; }
        public TierDivisionEnum Rank { get; set; }

        public override bool Equals(object obj)
        {
            var view = obj as PartialSummonerView;
            return view != null &&
                   SummonerName == view.SummonerName &&
                   RoleForTeam == view.RoleForTeam &&
                   Rank == view.Rank;
        }

        public override int GetHashCode()
        {
            var hashCode = -772324406;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SummonerName);
            hashCode = hashCode * -1521134295 + RoleForTeam.GetHashCode();
            hashCode = hashCode * -1521134295 + Rank.GetHashCode();
            return hashCode;
        }
    }
}
