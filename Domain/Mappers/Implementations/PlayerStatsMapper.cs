using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Entities.LeagueInfo;
using Domain.Mappers.Interfaces;
using Domain.Views;

namespace Domain.Mappers.Implementations
{
    public class PlayerStatsMapper : IPlayerStatsMapper
    {
        public PlayerStatsView Map(PlayerStatsEntity entity)
        {
            return new PlayerStatsView
            {
                SummonerId = entity.SummonerId,
                Kills = entity.Kills,
                Deaths = entity.Deaths,
                Assists = entity.Assists,
                Kda = Math.Round((entity.Kills + entity.Assists)/(float)entity.Deaths, 2),
                CSperMin = Math.Round(entity.CS/(float)entity.GameTime.TotalMinutes, 2),
                DamagePerMin = Math.Round(entity.Gold/(float)entity.GameTime.TotalMinutes, 2),
                Kp = Math.Round((entity.Kills + entity.Assists) / (float)entity.TotalTeamKills * 100, 1),
                VisionScore = Math.Round(entity.VisionScore / ((float)entity.Games * 100) * 100, 1),
                SeasonInfoId = entity.SeasonInfoId
            };
        }

        public IEnumerable<PlayerStatsView> Map(IEnumerable<PlayerStatsEntity> entities)
        {
            return entities.Select(Map);
        }

        public PlayerStatsView MapForSeason(IEnumerable<PlayerStatsEntity> entities)
        {
            var view = new PlayerStatsView();
            var entityList = entities.ToList();
            if (entityList.Any())
            {
                var first = entityList.First();
                for (var i = 1; i < entityList.Count; i++)
                {
                    var entity = entityList[i];
                    first += entity;
                }
                view = Map(first);
            }
            
            return view;
        }
    }
}
