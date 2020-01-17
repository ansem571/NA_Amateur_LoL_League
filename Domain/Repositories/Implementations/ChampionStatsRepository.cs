using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Implementations
{
    public interface IChampionStatsRepository
    {
        Task<IEnumerable<ChampionStatsEntity>> GetAllChampionStats();
        Task<IEnumerable<ChampionStatsEntity>> GetStatsForPlayerAsync(Guid playerId);
        Task<IEnumerable<ChampionStatsEntity>> GetChampionStatsForPlayerAsync(Guid playerId, Guid championId);
        Task<bool> CreateAsync(IEnumerable<ChampionStatsEntity> entities);
        Task<bool> DeleteByScheduleAsync(Guid teamScheduleId);
    }
    public class ChampionStatsRepository : IChampionStatsRepository
    {
        private readonly ITableStorageRepository<ChampionStatsEntity> _table;

        public ChampionStatsRepository(ITableStorageRepository<ChampionStatsEntity> table)
        {
            _table = table ?? throw new NotImplementedException(nameof(table));
        }
        
        /// <summary>
        /// would rather not use
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ChampionStatsEntity>> GetAllChampionStats()
        {
            var entities = await _table.ReadAllAsync();
            return entities;
        }

        public async Task<IEnumerable<ChampionStatsEntity>> GetStatsForPlayerAsync(Guid playerId)
        {
            var entities = await _table.ReadManyAsync("PlayerId = @playerId", new {playerId});
            return entities;
        }

        public async Task<IEnumerable<ChampionStatsEntity>> GetChampionStatsForPlayerAsync(Guid playerId, Guid championId)
        {
            var entities = await _table.ReadManyAsync("PlayerId = @playerId AND ChampionId = @championId", new { playerId, championId });
            return entities;
        }

        public async Task<bool> CreateAsync(IEnumerable<ChampionStatsEntity> entities)
        {
            entities = entities.ToList();
            if (entities.Any())
            {
                return await _table.InsertAsync(entities) == entities.Count();
            }

            return true;
        }


        public async Task<bool> DeleteByScheduleAsync(Guid teamScheduleId)
        {
            var entities = await _table.ReadManyAsync("TeamScheduleId = @teamScheduleId", new {teamScheduleId});
            if (!entities.Any())
            {
                return true;
            }
            return await _table.DeleteWhereAsync("TeamScheduleId = @teamScheduleId", new {teamScheduleId});
        }
    }
}
