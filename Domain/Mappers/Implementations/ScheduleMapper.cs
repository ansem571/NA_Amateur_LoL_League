using System;
using DAL.Entities.LeagueInfo;
using Domain.Mappers.Interfaces;
using Domain.Views;

namespace Domain.Mappers.Implementations
{
    public class ScheduleMapper : IScheduleMapper
    {
        public ScheduleView Map(ScheduleEntity entity, string homeTeam, string awayTeam)
        {
            return new ScheduleView
            {
                ScheduleId = entity.Id,
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
                HomeTeamScore = entity.HomeTeamWins,
                AwayTeamScore = entity.AwayTeamWins,
                WeekOf = entity.MatchWeek,
                PlayTime = entity.MatchScheduledTime,
                CasterName = entity.CasterName,
                IsPlayoffMatch = entity.IsPlayoffMatch
            };
        }

        public ScheduleEntity Map(ScheduleView view, Guid seasonInfoId, Guid homeTeam, Guid awayTeam)
        {
            return new ScheduleEntity
            {
                Id = view.ScheduleId,
                SeasonInfoId = seasonInfoId,
                HomeRosterTeamId = homeTeam,
                AwayRosterTeamId = awayTeam,
                HomeTeamWins = view.HomeTeamScore,
                AwayTeamWins = view.AwayTeamScore,
                MatchWeek = view.WeekOf,
                MatchScheduledTime = view.PlayTime,
                CasterName = view.CasterName,
                IsPlayoffMatch = view.IsPlayoffMatch
            };
        }
    }
}
