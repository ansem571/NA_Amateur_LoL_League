using System.Collections.Generic;
using Domain.Views;

namespace Web.Models.Roster
{
    public class RosterViewModel
    {
        public bool IsCaptain { get; set; }
        public RosterView RosterView { get; set; }
        public string StatusMessage { get; set; }
        public IEnumerable<ScheduleView> ScheduleLineup { get; set; }
    }
}
