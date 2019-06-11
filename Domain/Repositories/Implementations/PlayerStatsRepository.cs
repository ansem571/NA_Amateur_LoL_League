using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class PlayerStatsRepository : IPlayerStatsRepository
    {
        private readonly ITableStorageRepository<PlayerStatsEntity> _table;

        public PlayerStatsRepository(ITableStorageRepository<PlayerStatsEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<PlayerStatsEntity> GetStatsForSummonerAsync(Guid summonerId)
        {
            var entity = (await _table.ReadManyAsync("SummonerId = @summonerId", new { summonerId }, top: 1))
                .FirstOrDefault();
            return entity;
        }

        public async Task<IEnumerable<PlayerStatsEntity>> GetStatsForSummonersAsync(IEnumerable<Guid> summonerIds)
        {
            var entities = await _table.ReadManyAsync("SummonerId in @summonerIds", new { summonerIds });
            return entities;
        }

        public async Task<IEnumerable<PlayerStatsEntity>> GetAllStatsAsync()
        {
            return await _table.ReadAllAsync();
        }

        public async Task<bool> InsertAsync(IEnumerable<PlayerStatsEntity> stats)
        {
            stats = stats.ToList();
            if (!stats.Any())
            {
                return true;
            }

            return await _table.InsertAsync(stats) == stats.Count();
        }

        public async Task<bool> UpdateAsync(IEnumerable<PlayerStatsEntity> stats)
        {
            stats = stats.ToList();
            if (!stats.Any())
            {
                return true;
            }

            return await _table.UpdateAsync(stats);
        }

        public async Task<bool> DeleteAsync(IEnumerable<PlayerStatsEntity> stats)
        {
            stats = stats.ToList();
            if (!stats.Any())
            {
                return true;
            }

            return await _table.DeleteAsync(stats);
        }
    }
}
