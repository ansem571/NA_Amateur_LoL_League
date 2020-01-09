using System;

namespace Domain.Helpers
{
    public class MatchDetailKey
    {
        public Guid ScheduleId { get; set; }
        public int Game { get; set; }
        public Guid PlayerId { get; set; }

        public MatchDetailKey()
        {
        }

        public MatchDetailKey(Guid scheduleId, int game, Guid playerId)
        {
            ScheduleId = scheduleId;
            Game = game;
            PlayerId = playerId;
        }
    }

    public class StatsKey
    {
        public Guid SummonerId { get; set; }
        public Guid SeasonId { get; set; }

        public StatsKey()
        {
        }

        public StatsKey(Guid summonerId, Guid seasonId)
        {
            SummonerId = summonerId;
            SeasonId = seasonId;
        }
    }

    public class MatchMvpKey
    {
        public Guid TeamScheduleId { get; set; }
        public int Game { get; set; }

        public MatchMvpKey() { }
        public MatchMvpKey(Guid teamScheduleId, int game)
        {
            TeamScheduleId = teamScheduleId;
            Game = game;
        }
    }
}
