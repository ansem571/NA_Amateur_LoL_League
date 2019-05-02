using System;
using System.Collections.Generic;
using System.Text;
using DAL.Entities.LeagueInfo;
using Domain.Views;

namespace Domain.Mappers.Interfaces
{
    public interface IPlayerStatsMapper
    {
        PlayerStatsView Map(PlayerStatsEntity entity);
        IEnumerable<PlayerStatsView> Map(IEnumerable<PlayerStatsEntity> entities);
    }
}
