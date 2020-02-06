using System.Collections.Generic;
using DAL.Entities.LeagueInfo;
using Domain.Views;

namespace Domain.Mappers.Interfaces
{
    public interface IPlayerStatsMapper
    {
        PlayerStatsView Map(PlayerStatsEntity entity, double points);
        PlayerStatsView MapForSeason(IEnumerable<PlayerStatsEntity> entities, double points);
    }
}
