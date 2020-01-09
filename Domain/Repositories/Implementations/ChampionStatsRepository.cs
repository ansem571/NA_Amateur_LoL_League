using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        Task<bool> DeleteForMatchDetailsAsync(IEnumerable<Guid> matchDetailIds);
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

        public async Task<bool> DeleteForMatchDetailsAsync(IEnumerable<Guid> matchDetailIds)
        {
            matchDetailIds = matchDetailIds.ToList();
            if (matchDetailIds.Any())
            {
                var result = await _table.DeleteWhereAsync("MatchDetailId in @matchDetailIds", new {matchDetailIds});
                return result;
            }

            return true;
        }
    }
}
