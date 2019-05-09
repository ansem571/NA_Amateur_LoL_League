using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Views
{
    public class ScheduleView
    {
        public Guid ScheduleId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public DateTime WeekOf { get; set; }
        public DateTime? PlayTime { get; set; }
        public string CasterName { get; set; }
    }
}
