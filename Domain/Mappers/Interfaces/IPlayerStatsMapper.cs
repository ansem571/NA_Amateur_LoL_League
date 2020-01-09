using System;
using System.Collections.Generic;
using DAL.Entities.LeagueInfo;
using Domain.Views;

namespace Domain.Mappers.Interfaces
{
    public interface IPlayerStatsMapper
    {
        PlayerStatsView Map(PlayerStatsEntity entity);
        [Obsolete("Use MapForSeason", true)]
        IEnumerable<PlayerStatsView> Map(IEnumerable<PlayerStatsEntity> entities);
        PlayerStatsView MapForSeason(IEnumerable<PlayerStatsEntity> entities);

    }
}
