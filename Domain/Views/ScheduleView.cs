using System;

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
        public bool IsPlayoffMatch { get; set; }

        public ScheduleView() { }

        public ScheduleView(string home, string away, DateTime week)
        {
            ScheduleId = Guid.NewGuid();
            HomeTeam = home;
            AwayTeam = away;
            HomeTeamScore = 0;
            AwayTeamScore = 0;
            WeekOf = week;
        }
    }
}
