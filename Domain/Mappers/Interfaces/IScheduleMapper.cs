using System;
using System.Collections.Generic;
using System.Text;
using DAL.Entities.LeagueInfo;
using Domain.Views;

namespace Domain.Mappers.Interfaces
{
    public interface IScheduleMapper
    {
        ScheduleView Map(ScheduleEntity entity, string homeTeam, string awayTeam);
        ScheduleEntity Map(ScheduleView view, Guid seasonInfoId, Guid homeTeam, Guid awayTeam);
    }
}
