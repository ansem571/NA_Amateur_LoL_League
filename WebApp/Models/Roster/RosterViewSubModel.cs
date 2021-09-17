using System.Collections.Generic;
using Domain.Views;

namespace Web.Models.Roster
{
    public class RosterViewSubModel
    {
        public IEnumerable<DetailedSummonerInfoView> Players { get; set; }

        public RosterViewSubModel() { }

        public RosterViewSubModel(IEnumerable<DetailedSummonerInfoView> players)
        {
            Players = players;
        }
    }
}
