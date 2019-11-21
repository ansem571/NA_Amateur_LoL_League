using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
