using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Views
{
    public class MatchInfo
    {
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public Guid ScheduleId { get; set; }
        public List<GameInfo> GameInfos { get; set; }
        public List<GameDetail> GameDetails { get; set; }
    }
}
