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
        public List<SummonerRoleEnum> LookingFor => LookingForRoles();
        public string LookingForString => string.Join(", ", LookingFor);

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
