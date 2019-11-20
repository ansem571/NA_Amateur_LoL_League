using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Helpers
{
    public class MatchDetailKey
    {
        public MatchDetailKey()
        {
        }

        public MatchDetailKey(Guid scheduleId, int game, Guid playerId)
        {
            ScheduleId = scheduleId;
            Game = game;
            PlayerId = playerId;
        }

        public Guid ScheduleId { get; set; }
        public int Game { get; set; }
        public Guid PlayerId { get; set; }
    }
}
