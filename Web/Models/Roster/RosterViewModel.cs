using System.Collections.Generic;
using System.Linq;
using Domain.Views;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace Web.Models.Roster
{
    public class RosterViewModel
    {
        public bool IsCaptain { get; set; }
        public RosterView RosterView { get; set; }
        public string StatusMessage { get; set; }
        public IEnumerable<ScheduleView> ScheduleLineup { get; set; }
        public string TeamOpGg => Setup();

        private string Setup()
        {
            var str = "https://na.op.gg/summoner/userName=";

            var urls = RosterView.Players.Select(x => x.OpGgUrl).ToList();
            foreach (var url in urls)
            {
                if (url == null)
                {
                    continue;
                }
                var urlSecure = url.Replace("http:", "https:");
                var urlName = urlSecure.Replace("https://na.op.gg/summoner/userName=", "");
                str += $"{urlName},";
            }

            str = str.Substring(0, str.Length - 1);
            return str;
        }
    }
}
