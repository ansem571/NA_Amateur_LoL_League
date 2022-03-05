using System.Collections.Generic;
using System.Linq;
using Domain.Views;

namespace Web.Models.Roster
{
    public class RosterViewModel
    {
        public bool IsCaptain { get; set; }
        public RosterView RosterView { get; set; }
        public string StatusMessage { get; set; }
        public IEnumerable<ScheduleView> ScheduleLineup { get; set; }
        public string TeamOpGg => Setup();
        public RosterViewSubModel Roster => new RosterViewSubModel(RosterView.Players);

        private string Setup()
        {
            var str = "https://na.op.gg/multisearch/na?summoners=";

            var summonerNames = RosterView.Players.Select(x => x.SummonerName).ToList();
            foreach (var summonerName in summonerNames)
            {

                str += $"{summonerName},";
            }

            str = str[0..^1];
            return str;
        }
    }
}
